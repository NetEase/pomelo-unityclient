#define LUZEXI
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;
using SimpleJson;

namespace pomeloUnityClient
{
	public class EventManager : IDisposable
	{
		
		private  Dictionary<int, Action<JsonObject>> callBackMap;
		private  Dictionary<string, List<Action<JsonObject>>> eventMap;

		public EventManager()
		{
			this.callBackMap = new  Dictionary<int, Action<JsonObject>>();
			this.eventMap = new  Dictionary<string, List<Action<JsonObject>>>();
		}
		
		//Adds callback to callBackMap by id.
		public void AddCallBack(int id, Action<JsonObject>  callback)
		{
			if (id != null && callback != null) {
				this.callBackMap.Add(id, callback);
			}
		}
		
		/// <summary>
		/// Invoke the callback when the server return messge .
		/// </summary>
		/// <param name='pomeloMessage'>
		/// Pomelo message.
		/// </param>
		public void InvokeCallBack(JsonObject msg)
		{
			if (msg != null) {
				Action<JsonObject>  action = null;
				object id = null;
				object body = null;
				if (msg.TryGetValue("id", out id)){
					if(this.callBackMap.TryGetValue(Convert.ToInt32(id), out action)) {
						if (msg.TryGetValue("body", out body)) {
							action.Invoke((JsonObject)SimpleJson.SimpleJson.DeserializeObject(body.ToString()));
						}
					}
				}
			}	
		}
		
		//Adds the event to eventMap by name.
		public void AddOnEvent(string eventName, Action<JsonObject> callback)
		{
			List<Action<JsonObject>> list = null;
			if (this.eventMap.TryGetValue(eventName, out list)) {
				list.Add(callback);
			} else {
				list = new List<Action<JsonObject>>();
				list.Add(callback);
				this.eventMap.Add(eventName, list);
			}
		}
		
		/// <summary>
		/// If the event exists,invoke the event when server return messge.
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		///
		public void InvokeOnEvent (JsonObject msg) {
			List<Action<JsonObject>> list = null;
			object route = null;
			if (msg.TryGetValue("route", out route)) {
				if (this.eventMap.TryGetValue(route.ToString(), out list)) {
					int length = list.Count;
					for(int i = 0; i < length; i++) {
						Action<JsonObject> ap = list[i];
						ap.Invoke(msg);
					}
				}
			}
		}

#if LUZEXI
		/// <summary>
		/// Gets the event.
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="eventName">Event name.</param>
		public List<Action<JsonObject>> GetEvent( string eventName )
		{
			List<Action<JsonObject>> lst = new List<Action<JsonObject>>();
			
			if( this.eventMap.ContainsKey(eventName))
			{
				lst = eventMap[eventName];
			}
			return lst;
		}
#endif

		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected void Dispose(bool disposing)
		{
			this.callBackMap.Clear();
			this.eventMap.Clear();
		}
	}
}

