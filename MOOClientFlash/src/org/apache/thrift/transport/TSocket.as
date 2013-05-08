/*
* Licensed to the Apache Software Foundation (ASF) under one
* or more contributor license agreements. See the NOTICE file
* distributed with this work for additional information
* regarding copyright ownership. The ASF licenses this file
* to you under the Apache License, Version 2.0 (the
* "License"); you may not use this file except in compliance
* with the License. You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied. See the License for the
* specific language governing permissions and limitations
* under the License.
*/

package org.apache.thrift.transport
{
	import flash.errors.IOError;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.events.ProgressEvent;
	import flash.net.Socket;

	public class TSocket extends TIOStreamTransport
	{
		
		private var socket:Socket = null;
		private var host:String = "localhost";
		private var port:int = 9090;		
		
		/**
		 * Constructor that takes an already created socket or host and remote port.
		 *
		 * @param socket Already created socket object - null if host and port are used
		 * @param host    Remote host
		 * @param port    Remote port
		 * @param timeout Socket timeout
		 * @throws TTransportException if there is an error setting up the streams
		 */
		public function TSocket(socket:Socket=null,host:String=null, port:int=0)
		{
			if(socket) 
			{
				this.socket = socket;	
				throw new Error("Supported only in Adobe Air");
				//this.host = socket.remoteAddress; 
				//this.port = socket.remotePort;
				inputStream = socket;
				outputStream = socket;								
			}
			else if(host && port > 0)
			{
				this.host = host;
				this.port = port;				
				initSocket();				
			}
		}		
		
		/**
		 * Initializes the socket object
		 */
		private function initSocket():void
		{
			try
			{
				socket = new Socket(host,port);	
				inputStream = socket;			
				outputStream = socket;
			}
			catch(e:IOError)
			{
				throw new TTransportError(TTransportError.IO_ERROR,"Unable to open Socket");            
			}
		}
		
		/**
		 * Returns a reference to the underlying socket.
		 */
		public function getSocket():Socket {
			if (socket == null) {
				initSocket();
			}
			return socket;
		}
		
		public override function isOpen():Boolean {
			if(this.socket == null) {
				return false;
			}
			else {
				return this.socket.connected;
			}
		}
		
		public override function open():void {
			if(isOpen()) {
				throw new TTransportError(TTransportError.ALREADY_OPEN, "Socket already connected");
			}
			
			if(host.length < 1) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Cannot connect without host");	
			}
			
			if(port <= 0) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Cannot connect without port");	
			}
			
			if(socket == null) {
				initSocket();
			}
			// setup all event listeners
			socket.addEventListener(ProgressEvent.SOCKET_DATA,dataReceivedHandler);
			socket.addEventListener(IOErrorEvent.IO_ERROR,ioErrorHandler);   
			socket.addEventListener(Event.CLOSE,socketClosedHandler);			
			
			try {
				socket.connect(host,port);
			}
			catch(error:IOError) {
				throw new TTransportError(TTransportError.IO_ERROR,"Can not connect to host: " + error.message);
			}
			catch(error:SecurityError) {
				throw new TTransportError(TTransportError.UNKNOWN,"Can not connect to host: " + error.message);
			}			
		}
		
		public override function close():void {
			if(socket != null) {				
				socket.close();
				socket.dispatchEvent(new Event(Event.CLOSE));				
			}
		}
		
		public override function flush():void {
			if(socket == null) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Stream is null");
			}	
			try {	
				socket.flush();
			}
			catch(error:IOError)
			{
				throw new TTransportError(TTransportError.IO_ERROR, error.message);
			}
		}
		
		/**
		 * This function is called each time the socket receives data.
		 * The TSocket class forwards the event.
		 * @param event ProgressEvent that contains information about the received data
		 **/
		public function dataReceivedHandler(event:ProgressEvent):void
		{
			this.dispatchEvent(event);			
		}
        
		/**
		 * This function is called if the socket encounters an IO Error. 
		 * This happens for example if no connection to the remote host can be established.
		 * @param event IOErrorEvent that contains information about the error.
		 **/ 
        public function ioErrorHandler(event:IOErrorEvent):void
        {
			// Close socket on all errors
            socket.close();			
			// Make sure the socketClosedHandler function is called
			socket.dispatchEvent(new Event(Event.CLOSE));			
        }
		
		/**
		 * This function is called when the socket gets closed
		 * @param event Contains the Event information
		 **/
		public function socketClosedHandler(event:Event):void
		{
			// remove all event listeners
			socket.removeEventListener(ProgressEvent.SOCKET_DATA,dataReceivedHandler);
			socket.removeEventListener(IOErrorEvent.IO_ERROR,ioErrorHandler);
			socket.removeEventListener(Event.CLOSE,socketClosedHandler);
			super.close();
			// propagate the closed event to higher level transports or application
			this.dispatchEvent(new TTransportEvent(TTransportEvent.T_TRANSPORT_CLOSED));
		}
	}
}