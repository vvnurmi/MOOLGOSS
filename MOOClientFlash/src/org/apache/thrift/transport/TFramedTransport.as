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
	import flash.errors.EOFError;
	import flash.errors.IOError;	
	import flash.events.ProgressEvent;	
	import flash.net.Socket;	
	import flash.utils.ByteArray;
	import flash.utils.IDataInput;
	import flash.utils.IDataOutput;
	
	import org.apache.thrift.TAsyncResult;
	
	public class TFramedTransport extends TTransport
	{
		/**
		 * Underlying Transport
		 */
		private var transport:TTransport;
			
		private var maxLength:uint;	
		private var writeBuffer:ByteArray;
		private var readBuffer:ByteArray;		
			
		private var receiveState:uint = RECEIVE_HEADER;
		private var receivedSize:uint;		
		
		private static const RECEIVE_HEADER:uint = 0;
		private static const RECEIVE_DATA:uint = 1;
			

		/**
		 * Constructor wraps around another transport
		 */
		public function TFramedTransport(transport:TTransport, maxLength:int=1024):void {
			this.transport = transport;
			transport.addEventListener(ProgressEvent.SOCKET_DATA,dataReceivedHandler);
			transport.addEventListener(TTransportEvent.T_TRANSPORT_CLOSED,transportClosedAndErrorHandler);
			transport.addEventListener(TTransportEvent.T_TRANSPORT_ERROR,transportClosedAndErrorHandler);
			
			this.maxLength = maxLength;
			writeBuffer = new ByteArray();
			// dummy header
			writeBuffer.writeInt(0);
			readBuffer = new ByteArray();
		}
		
		
		public override function open():void {
			transport.open();				
		}
		
		public override function close():void {
			transport.close();
		}
		
		public override function isOpen():Boolean {
			return transport.isOpen();
		}
				
		public override function read(buf:ByteArray, off:int, len:int):int {
			if (readBuffer != null) {
				if(readBuffer.bytesAvailable >= len)
				{
					readBuffer.readBytes(buf, off, len);
					return len;
				}
				else
				{
					var got:int = readBuffer.bytesAvailable;
					readBuffer.readBytes(buf, off, readBuffer.bytesAvailable);
					return got;
				}				
			}
			
			return 0;					
		}		
		
		public override function write(buf:ByteArray, off:int, len:int):void {						
			writeBuffer.writeBytes(buf,off,len);
		}		
	
		/**
		* Creates a new packet(frame) from all data in the write buffer, adds frame length information and flushes the complete frame into the underlying transport.
		**/
		public override function flush():void {
			var len:uint = writeBuffer.length;
					
			// Inject message header into the reserved buffer space			
			writeBuffer.position = 0;
			writeBuffer.writeUnsignedInt(len-4);
			// write to transport below
			transport.write(writeBuffer, 0, len);
			transport.flush();
			// Reset write buffer
			writeBuffer.position = 4;
			writeBuffer.length = 4;			
		}
		
		/**
		 * Is called when data was received from the transport.
		 * The data is then interpretted and the appropriate callback called
		 */
		public function dataReceivedHandler(e:ProgressEvent):void
		{
			// Read all bytes from the transport
			transport.readAll(readBuffer,readBuffer.length,e.bytesLoaded);
			
			while(1)
			{
				if(receiveState == RECEIVE_HEADER)
				{
					if(readBuffer.length >= 4) // read the frame header
					{
						receivedSize = readBuffer.readUnsignedInt();						
						receiveState = RECEIVE_DATA;
					}
					else break;
				}
				
				if(receiveState == RECEIVE_DATA)
				{
					if(readBuffer.length >= (receivedSize + 4)) // frame is complete, read the data
					{
						readBuffer.position=4;
						var receivedMessage:ByteArray = new ByteArray();
						readBuffer.readBytes(receivedMessage,0,receivedSize);
						readBuffer.position=4;
						
						// Dispatch Message that a complete frame/message was received
						this.dispatchEvent(new TTransportEvent(TTransportEvent.T_MESSAGE_RECEIVED));
						// Here the message hopefully gets interpreted by the processor :-)
						
						// Skip message. Only necessary for safety if there is no processor that reads the data
						readBuffer.position=4+receivedSize;
												
						// Drop message by copying remaining bytes to the receive buffer
						var newBuffer:ByteArray = new ByteArray();
						readBuffer.readBytes(newBuffer,0,0);
						newBuffer.position = 0;
						readBuffer = newBuffer;
						
						receiveState = RECEIVE_HEADER;	
					}
					else break;
				}	
			}						
		}
		
		public function transportClosedAndErrorHandler(event:TTransportEvent):void
		{			
			// Forward the event
			this.dispatchEvent(event);			
		}
		
	}
}