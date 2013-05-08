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

package org.apache.thrift.transport {

	import flash.errors.EOFError;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.events.SecurityErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLLoaderDataFormat;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.system.Capabilities;
	import flash.utils.ByteArray;
  
	 /**
	 * HTTP implementation of the TTransport interface. Used for working with a
	 * Thrift web services implementation.
	 */
	public class THttpClient extends TTransport {
	
		private var request_:URLRequest = null;
		private var requestBuffer_:ByteArray = new ByteArray();
		private var responseBuffer_:ByteArray = null;
		private var traceBuffers_:Boolean = Capabilities.isDebugger;
	
		public function THttpClient(request:URLRequest, traceBuffers:Boolean=true):void {
			request.contentType = "application/x-thrift";
			request_ = request;
			if(traceBuffers == false) {
				traceBuffers_ = traceBuffers;
			}
		}
	    
		public function getBuffer():ByteArray {
	  		return requestBuffer_;
		}		
	    
		public override function open():void {
		}
		
		public override function close():void {
		}
		 
		public override function isOpen():Boolean {
			return true;
		}
	    
		public override function read(buf:ByteArray, off:int, len:int):int {
			if (responseBuffer_ == null) {
				throw new TTransportError(TTransportError.UNKNOWN, "Response buffer is empty.");
			}
			try {
				responseBuffer_.readBytes(buf, off, len);
				if (traceBuffers_) {
					dumpBuffer(buf, "READ");
				}
				return len;
			}
			catch (e:EOFError) {
				if (traceBuffers_) {
					dumpBuffer(responseBuffer_, "FAILED-RESPONSE");
				}
				throw new TTransportError(TTransportError.UNKNOWN, "No more data available.");
			}
			return 0;
		}
	
		public override function write(buf:ByteArray, off:int, len:int):void {
			requestBuffer_.writeBytes(buf, off, len);
		}
	
		public override function flush():void {
			var loader:URLLoader = new URLLoader();
			loader.dataFormat = URLLoaderDataFormat.BINARY;
			
			// Setup callbacks
			loader.addEventListener(Event.COMPLETE, function(event:Event):void {
				responseBuffer_ = URLLoader(event.target).data;
				if (traceBuffers_) dumpBuffer(responseBuffer_, "RESPONSE_BUFFER");				
				dispatchEvent(new TTransportEvent(TTransportEvent.T_MESSAGE_RECEIVED));
				// Here the message hopefully gets interpreted by the processor :-)
				responseBuffer_ = null;								
			});			
			loader.addEventListener(IOErrorEvent.IO_ERROR, function(event:IOErrorEvent):void {
				dispatchEvent(new TTransportEvent(TTransportEvent.T_TRANSPORT_ERROR,"IO Error: " + event.text));				
				responseBuffer_ = null;
			});
			loader.addEventListener(SecurityErrorEvent.SECURITY_ERROR, function(event:SecurityErrorEvent):void {
				dispatchEvent(new TTransportEvent(TTransportEvent.T_TRANSPORT_ERROR,"Security Error: " + event.text));				
				responseBuffer_ = null;
			});
			
			// Setup data
			requestBuffer_.position = 0;
			request_.method = URLRequestMethod.POST;			
			request_.data = requestBuffer_;
			
			// Send the complete request
			loader.load(request_);
			// Clear the request buffer
			requestBuffer_.clear();			
		}
	
		private function dumpBuffer(buf:ByteArray, prefix:String):String {
			var debugString : String = prefix + " BUFFER ";
			if (buf != null) {
				debugString += "length: " + buf.length + ", ";
				for (var i : int = 0; i < buf.length; i++) {
					debugString += "[" + buf[i].toString(16) + "]";
				}
			} else {
				debugString = "null";
			}
			trace(debugString);
			return debugString;
		}

	}
}
