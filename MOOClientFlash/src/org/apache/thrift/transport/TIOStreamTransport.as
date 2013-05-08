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
	import flash.utils.ByteArray;
	import flash.utils.IDataInput;
	import flash.utils.IDataOutput;	

	public class TIOStreamTransport extends TTransport
	{
		/** Underlying inputStream */
		protected var inputStream:IDataInput = null;
		
		/** Underlying outputStream */
		protected var outputStream:IDataOutput = null;

		public function TIOStreamTransport() {

		}
		
		public function setStreams(inputStream:IDataInput, outputStream:IDataOutput):void {
			this.inputStream = inputStream;
			this.outputStream = outputStream;
		}
		
		public function getOutputStream():IDataOutput {
			return outputStream;
		}
		
		public function getInputStream():IDataInput {
			return inputStream;
		}
		
		/**
		 * The streams must already be open at construction time, so this should
		 * always return true.
		 *
		 * @return true
		 */
		public override function isOpen():Boolean {
			return true;
		}
		
		/**
		 * The streams must already be open. This method does nothing.
		 */
		public override function open():void {				
		}
		
		/**
		 * Closes both the input and output streams.
		 */
		public override function close():void {
			if(inputStream != null) {
				inputStream = null;
			}
			if(outputStream != null) {
				outputStream = null;
			}
		}
		
		/**
		 * Reads from the underlying input stream if not null.
		 */
		public override function read(buf:ByteArray, off:int, len:int):int {
			if(inputStream == null) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Input Stream is null");
			}
			try {	
				inputStream.readBytes(buf, off, len);
				return len;
			}
			catch(error:EOFError) {
				throw new TTransportError(TTransportError.END_OF_FILE, "Cannot read. End of File. Tried to read " + len + " bytes");
			}
			catch(error:IOError)
			{
				throw new TTransportError(TTransportError.IO_ERROR, "Cannot read. Remote side has closed. Tried to read " + len + " bytes.\n" + error.message);
			}
			return 0;
		}
		
		/**
		 * Writes to the underlying output stream if not null.
		 */
		public override function write(buf:ByteArray, off:int, len:int):void {
			if(outputStream == null) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Output Stream is null");
			}	
			try {	
				outputStream.writeBytes(buf,off,len);
			}
			catch(error:IOError)
			{
				throw new TTransportError(TTransportError.IO_ERROR, "Cannot write. Remote side has closed. Tried to write " + len + " bytes.\n" + error.message);
			}
		}
		
		/**
		 * Flushes the underlying output stream if not null.
		 */
		public override function flush():void {
			if(outputStream == null) {
				throw new TTransportError(TTransportError.NOT_OPEN, "Stream is null");
			}			
		}
	}
}