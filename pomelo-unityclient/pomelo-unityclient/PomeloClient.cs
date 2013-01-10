using System;
using System.Text;
using System.Diagnostics;
using SimpleJson;
using System.Collections.Generic;
using SocketIOClient;

namespace pomeloUnityClient
{
	public class PomeloClient
	{
		private Client socket = null; 
		private int reqId = 0;
		private const string ARRAY_FLAG = "[";
		private const string URL_HEADER = "http://";
		private EventManager eventManager = null;
		
		public PomeloClient (string url)
		{
			string checkedUrl = this.checkUrl(url);
			
			try{
				this.socket = new Client(checkedUrl);
			} catch (Exception e) {
				Console.WriteLine(string.Format("Error in new SocketIO:{0}", e.Message));
			}
			
			this.eventManager = new EventManager();
		}
		
		//Init socket and make connection.
		public void init(){
			
			this.socket.Opened += this.SocketOpened;
			this.socket.Message += this.SocketMessage;
			this.socket.SocketConnectionClosed += this.SocketConnectionClosed;
			this.socket.Error += this.SocketError;
			
			this.socket.Connect();
		}
		
		//Check out the url and complemented it.
		private string checkUrl(string url){
			string trueUrl;
			
			if (!url.Contains(URL_HEADER)) {
				 trueUrl = URL_HEADER + url;
			} else {
				trueUrl = url;
			}
			
			return trueUrl;	
		}
		
		//Close the socket and free the resources.
		public void disconnect(){
			this.socket.Close();
			if (this.eventManager != null) {
				this.eventManager.Dispose();
				this.eventManager = null;
			}
			this.closeSocketIO();
			
		}
		
		/// <summary>
		/// Sends the message to server.
		/// </summary>
		/// <param name='reqId'>
		/// Req identifier.
		/// </param>
		/// <param name='route'>
		/// Route.
		/// </param>
		/// <param name='msg'>
		/// Message.
		/// </param>
		private void sendMessage(int reqId, string route, JsonObject msg){
			string message = "";
			try{
				message = Protocol.encode(reqId, route, msg);
			}catch(ArgumentException e) {
				Console.WriteLine(string.Format("Error in protocol.encode:{0}", e.Message));
			}
			
			this.socket.Send(message);
		}
		
		/// <summary>
		/// Request message from server and register callback.
		/// </summary>
		/// <param name='route'>
		/// Route.
		/// </param>
		/// <param name='msg'>
		/// Message.
		/// </param>
		/// <param name='action'>
		/// Action.
		/// </param>
		public void request(string route, JsonObject msg, Action<JsonObject> action){
			
			JsonObject returnMSg = filter(msg);
			reqId++;
			this.eventManager.AddCallBack(reqId, action);
			this.sendMessage(reqId, route, returnMSg);
			
		}
		
		//Notify message to server.
		public void notify(string route, JsonObject msg) {
			this.sendMessage(0, route, msg);
		}
		
		//Listen evetn named eventName.
		public void On(string eventName, Action<JsonObject> action){
			this.eventManager.AddOnEvent(eventName, action);
		}
		
		//Add msg time.
		private JsonObject filter(JsonObject msg) {
			if (msg == null) {
				msg = new JsonObject();
			}
			var  st=  new DateTime(1970,1,1);
			TimeSpan t= (DateTime.Now.ToUniversalTime()-st);
			Int64  retval= (Int64)(t.TotalMilliseconds+0.5);
			msg.Add("timestamp", retval);
			return msg;
		}
		
		/// <summary>
		/// Processes the message and invoke callback or event.
		/// </summary>
		/// <param name='msg'>
		/// Message.
		/// </param>
		private void processMessage(string msg){
			JsonObject jsonMsg = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(msg);
			Object id = null;
			//-----------the request and notify message from server-----------------
			if (jsonMsg.TryGetValue("id", out id)) {
				this.eventManager.InvokeCallBack(jsonMsg);
			//------------bordcast message form server------------------------------
			} else {
				this.eventManager.InvokeOnEvent(jsonMsg);
			}
		}
		
		//Processes the message and invoke callback or event.
		private void processMessageBatch(string msgs){
			JsonArray jsonArray = (JsonArray) SimpleJson.SimpleJson.DeserializeObject(msgs); 
			int length = jsonArray.Count;
			for (int i = 0; i < length; i++) {
				this.processMessage(jsonArray[i].ToString());
			}
		}
		
		//Free the resources
		private void closeSocketIO(){
			if (this.socket != null) {
				this.socket.Opened -= this.SocketOpened;
				this.socket.Message -= this.SocketMessage;
				this.socket.SocketConnectionClosed -= this.SocketConnectionClosed;
				this.socket.Error -= this.SocketError;
				
				this.socket = null;
			}
		}
		
		//connection opened event.
		private void SocketOpened (object sender, EventArgs e){
			Console.WriteLine("The socketIO opend!");
		}
		/// <summary>
		/// When message from server comes, it invoke.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		private void SocketMessage (object sender, MessageEventArgs e) {
			
			if ( e!= null && e.Message.Event == "message") {
				string msg = e.Message.MessageText;
				if (msg.IndexOf(ARRAY_FLAG) == 0) {
					this.processMessageBatch(msg);
				} else {
					this.processMessage(msg);
				}
			}
		}
		
		//Connetction close event.
		private void SocketConnectionClosed (object sender, EventArgs e) {
			Console.WriteLine("WebSocketConnection was terminated!");
		}
		
		//Connection error event.
		private void SocketError (object sender, ErrorEventArgs e) {
			Console.WriteLine("socket client error:");
			Console.WriteLine(e.Message);
		}
	}
}

