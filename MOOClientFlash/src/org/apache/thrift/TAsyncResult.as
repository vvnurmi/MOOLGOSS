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

package org.apache.thrift
{
	/**
	* An asynchronous result object that is created for each RPC request. 
	* The result object contains the information which callback methods have to be called if the requests succeeds or throws an error.
	* It also stores the method that is used to decode the response message.
	**/
	public class TAsyncResult
	{
		/**
		* stores the receive function that has to be called to interpret a message response
		**/
		private var receiveMethod:Function = null;
	    
		/**
		* Method that is called if the request succeeds
		**/
		private var callback:Function = null;
	    
		/**
		* Method that is called on errors during the request
		**/
		private var errback:Function = null;
    
		/**
		* Creates a new TAsyncResult object
		* @param receiveMethod The method that has to be called to interpret the response to the sent request
		**/    
		public function TAsyncResult(receiveMethod:Function)
		{
			this.receiveMethod = receiveMethod;			
		}			

		/**
		* Returns the receive function that has to be called to interpret a message response
		**/
		public function get ReceiveMethod():Function
		{
			return receiveMethod;
		}				

		/**
		 * Method that is called if the request succeeds
		 **/
		public function get Callback():Function
		{
			return callback;
		}

		/**
		* @private
		**/
		public function set Callback(value:Function):void
		{
			callback = value;		
		}				
		
		/**
		 * Method that is called on errors during the request
		 **/
		public function get Errback():Function
		{
			return errback;
		}
		
		/**
		* @private
		**/
		public function set Errback(errback:Function):void
		{
			this.errback = errback;			
		}
	}
}