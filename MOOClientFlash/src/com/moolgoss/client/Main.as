package com.moolgoss.client
{
	import com.moolgoss.service.MOOServiceClient;
	import com.moolgoss.service.UpdateData;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.net.URLRequest;
	import flash.text.engine.TextBlock;
	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.text.TextFormatAlign;
	import flash.text.TextFieldAutoSize;
	import org.apache.thrift.protocol.TBinaryProtocol;
	import org.apache.thrift.protocol.TProtocol;
	import org.apache.thrift.TAsyncResult;
	import org.apache.thrift.TProcessor;
	import org.apache.thrift.transport.TFramedTransport;
	import org.apache.thrift.transport.TSocket;
	
	public class Main extends Sprite 
	{
		public function Main() : void 
		{
			if (stage) init();
			else addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e : Event = null) : void 
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			connectToServer("Foobar");
		}
		
		private function connectToServer(name : String) : void
		{
			var transport : TSocket = new TSocket(null, "localhost", 8000);
			transport.open();
			var framedTransport : TFramedTransport = new TFramedTransport(transport);
			var protocol : TProtocol = new TBinaryProtocol(framedTransport);
			var processor : TProcessor = new TProcessor(protocol);
			var client : MOOServiceClient = new MOOServiceClient(processor);
			client.send_Authenticate(name, null, onError);
			client.send_GetUpdate(onSuccess, onError);
		}
		
		private function onSuccess(updateData : UpdateData) : void
		{
			var helpText : TextField = new TextField();
			helpText.text = updateData.stardate.hour + ":" + updateData.stardate.minute;
			helpText.setTextFormat(new TextFormat("Comic", 20, 0x008000, false, true, false, null, null, TextFormatAlign.CENTER));
			helpText.autoSize = TextFieldAutoSize.CENTER;
			helpText.x = (stage.stageWidth - helpText.textWidth) / 2;
			helpText.y = 5;
			stage.addChild(helpText);
		}

		private function onError(error : Error) : void
		{
			var helpText : TextField = new TextField();
			helpText.text = "Error occurred: " + error.message;
			helpText.setTextFormat(new TextFormat("Comic", 20, 0xff0000, false, true, false, null, null, TextFormatAlign.CENTER));
			helpText.autoSize = TextFieldAutoSize.CENTER;
			helpText.x = (stage.stageWidth - helpText.textWidth) / 2;
			helpText.y = 5;
			stage.addChild(helpText);
		}
	}
	
}