using System;
using System.Text;
using SimpleJson;
using System.Diagnostics;

namespace pomeloUnityClient
{
	public class Protocol
	{
		private const int HEADER = 5; 
		
		public Protocol ()
		{
			
		}
		/// <summary>
		/// Encode the messge with id, route and jsonObject.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='route'>
		/// Route.
		/// </param>
		/// <param name='jsonObject'>
		/// Json object.
		/// </param>
		/// <exception cref='System.ArgumentException'>
		/// Is thrown when the argument exception.
		/// </exception>
		public static string encode(int id, string route, JsonObject jsonObject){
			
			if (route.Length > 255) {
				throw new System.ArgumentException("route maxlength is overflow");
			}
			
			byte[] byteArray = new byte[HEADER + route.Length];
			int index = 0;
			byteArray[index++] = Convert.ToByte((id >> 24) & 0xFF);
			byteArray[index++] = Convert.ToByte((id >> 16) & 0xFF);
			byteArray[index++] = Convert.ToByte((id >> 8) & 0xFF);
			byteArray[index++] = Convert.ToByte(id & 0xFF);
			byteArray[index++] = Convert.ToByte(route.Length & 0xFF);
			
			char[] routeArray = route.ToCharArray();
			int routeLength = routeArray.Length;
			for(int i = 0; i < routeLength; i++) {
				byteArray[index++] = Convert.ToByte(routeArray[i]);
			}
			string encodeString = "";
			try{
				encodeString = Encoding.UTF8.GetString(byteArray);
			}catch(Exception e){
				Console.WriteLine(string.Format("Error in new Encoding.UTF8.GetString:{0}", e.Message));
			}
			return encodeString + jsonObject.ToString();
		}		
		
	}
}

