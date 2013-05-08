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

package org.apache.thrift {

	import flash.events.Event;
	import flash.utils.Dictionary;
	
	import org.apache.thrift.*;
	import org.apache.thrift.meta_data.*;
	import org.apache.thrift.protocol.*;
	import org.apache.thrift.transport.*;

	/**
	 * A processor is a generic object which operates upon an input stream and
	 * writes to some output stream.  
	 * The processor registers at the underlying transport and intreprets received messages.  
	 * The processor stores information about all client side requests in the requestMap and can therefore call the corresponding receive methods if a response was received.
	 * It also stores information about all server functions that are provided by the object in a second dictionary and dispatches the client requests to the interface that serves the requests.
	*/
	public class TProcessor {
		/**
		* Dictionary that contains all Server functions. Key is the function name, value the function pointer
		**/
		protected const PROCESS_MAP:Dictionary = new Dictionary();
	
		/**
		* Dictionary that contains all client requests in form of a TAsyncRequest object.
		* The key of the dictionary is the sequence ID that is used for the request.
		**/
		public var requestMap:Dictionary = new Dictionary(true);
	
		/**
		* Last sequence id that was used for a client request
		**/
		public var seqid:int = 0;
		
		/**
		* Protocol that is used for reading data
		**/
		public var iprot:TProtocol;
		
		/** 
		* Protocol that is used for writing data
		**/
		public var oprot:TProtocol;    
	  
		/**
		* Creates a new processor that listens for received messages and interprets them
		* @param iprot The protocol that is used for reading data. The processor registers at the underlying transport and listens for messages.
		* @param oprot The protocol that is used for writing data. If null iprot is used.
		**/
		public function TProcessor(iprot:TProtocol, oprot:TProtocol=null)
		{
			this.iprot = iprot;
			if(oprot) this.oprot = oprot;
			else this.oprot = iprot;
			  
			// Register for incoming messages
			iprot.getTransport().addEventListener(TTransportEvent.T_MESSAGE_RECEIVED,process);     
			iprot.getTransport().addEventListener(TTransportEvent.T_TRANSPORT_ERROR,transportErrorHandler);
			iprot.getTransport().addEventListener(TTransportEvent.T_TRANSPORT_CLOSED,transportErrorHandler);
		}    

		/**
		* Processes the content of one received message. 
		* The function is triggered by an TMessageReceivedEvent 
		* @param event Event parameter
		* @throws TApplicationError if there is an error during message decoding
		**/ 
		public function process(event:Event):void
		{
			var msg:TMessage = iprot.readMessageBegin();
			if(msg.type == TMessageType.REPLY)
			{
				var ar:TAsyncResult = requestMap[msg.seqid];
				if(!ar) {
					TProtocolUtil.skip(iprot, TType.STRUCT);
					iprot.readMessageEnd();       
					throw new TApplicationError(TApplicationError.UNKNOWN_METHOD, "Invalid reply '" +msg.name+ "' with no matching sequence id " + msg.seqid.toString());
					return;
				}
				// call the receive function that is stored in the TAsyncRequest
				ar.ReceiveMethod(msg,ar);	
				// clean up Request dictionary
				requestMap[msg.seqid] = null;      
				delete requestMap[msg.seqid];
				return;
			}
			else // Message type is Call or Exception
			{
				var fn:Function = PROCESS_MAP[msg.name];
				if (fn == null) {
				TProtocolUtil.skip(iprot, TType.STRUCT);
				iprot.readMessageEnd();
				var x:TApplicationError = new TApplicationError(TApplicationError.UNKNOWN_METHOD, "Invalid method name: '"+msg.name+"'");
				oprot.writeMessageBegin(new TMessage(msg.name, TMessageType.EXCEPTION, msg.seqid));
				x.write(oprot);
				oprot.writeMessageEnd();
				oprot.getTransport().flush();
				return;
			}
			fn.call(this,msg.seqid, iprot, oprot);
			return;
			}		  
		}
		
		/**
		 * The function is called when the used transport is closed or encounters an error.<br>
		 * It cleans up the request map and signals an error to all pending requests.
		 * @param event The TTransportEvent that signals the transport error
		 **/
		public function transportErrorHandler(event:TTransportEvent):void
		{
			// Signal error on all pending requests
			for (var key:Object in requestMap) 
			{
				// fetch the request information
				var ar:TAsyncResult = requestMap[key];
				var errorMessage:String = "Request failed due to Transport error";
				if(event.message) errorMessage += ":\n" + event.message;
				// call error handler on all open requests
				ar.Errback(new TError(errorMessage));		
				// delete open requests
				requestMap[key] = null;      
				delete requestMap[key];			
			}			
		}	
		

	}

}