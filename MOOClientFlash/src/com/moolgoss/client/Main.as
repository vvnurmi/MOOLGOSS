package com.moolgoss.client
{
	import com.moolgoss.service.MOOService;
	import com.moolgoss.service.MOOServiceImpl;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.net.URLRequest;
	import org.apache.thrift.protocol.TBinaryProtocol;
	import org.apache.thrift.transport.THttpClient;
	
	[SWF(backgroundColor = "0x000000")]
	public class Main extends Sprite 
	{
		public function Main():void 
		{
			if (stage) init();
			else addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event = null) : void 
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			connectToServer("Foobar");
		}
		
		private function connectToServer(name : String) : MOOService
		{
			var transport : THttpClient = new THttpClient(new URLRequest("http://vs1164254.server4you.net:8000/"));
			var protocol : TBinaryProtocol = new TBinaryProtocol(transport);
			var client : MOOService = new MOOServiceImpl(protocol);
			transport.open();
			var success : Boolean = true;
			client.Authenticate(name, function(error : Error) : void { success = false; }, null);
			return success ? client : null;
		}
	}
	
}