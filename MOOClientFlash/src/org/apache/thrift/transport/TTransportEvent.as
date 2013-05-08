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
	import flash.events.Event;
	
	/**
	 * The TTransportEvent is used by Thrift Transports to signal information about the transports state to other layers
	 **/
	public class TTransportEvent extends Event
	{
		public static const T_MESSAGE_RECEIVED:String = "THRIFT_MESSAGE_RECEIVED";
		public static const T_TRANSPORT_CLOSED:String = "THRIFT_TRANSPORT_CLOSED";
		public static const T_TRANSPORT_ERROR:String  = "THRIFT_TRANSPORT_ERROR";
		
		/**
		 * Message that is carried within the event
		 **/
		public var message:String=null;
		
		/**
		 * Creates a new TTransport event
		 * @param type Event type
		 * @param message An optional message that is carried within the event
		 * @param bubbles Event bubbles
		 * @param cancelable Event is cancelable
		 **/		
		public function TTransportEvent(type:String, message:String=null, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			this.message = message;
			super(type, bubbles, cancelable);
		}		
		
		override public function clone():Event {
			return new TTransportEvent(type,message,bubbles,cancelable);
		}
		
		override public function toString():String {
			if(message) return message;
			else return super.toString();
		}
	}
}