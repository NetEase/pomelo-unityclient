using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJson;

//	TranspotUpdate.cs
//	Author: Lu Zexi
//	2014-6-20



namespace pomeloUnityClient
{
	/// <summary>
	/// Transpot updater.
	/// </summary>
	public class ClientUpdater : MonoBehaviour
	{
		private enum STATE
		{
			NONE = 0,
			START = 1,
			SOCKET_ERROR = 2,
			RUNING = 3,
			CLOSE = 4,
		}
		private STATE m_eStat = STATE.NONE;	//the state of the transpotUpdate
		private Action m_cUpdate;	//update action
		private Action m_cOnOpen;	//open event
		private Action m_cOnClose;	//close event
		private Action m_cOnSocketError;	//socket event

		/// <summary>
		/// Init this instance.
		/// </summary>
		internal static ClientUpdater Init( Action onConnect , Action onDisconnect , Action socketError , Action onUpdate )
		{
			GameObject obj = new GameObject("Socket");
			ClientUpdater trans = obj.AddComponent<ClientUpdater>();
			trans.m_cOnOpen = onConnect;
			trans.m_cOnClose = onDisconnect;
			trans.m_cOnSocketError = socketError;
			trans.m_cUpdate = onUpdate;
			return trans;
		}

		/// <summary>
		/// close the updater
		/// </summary>
		internal void _Close()
		{
			this.m_eStat = STATE.CLOSE;
		}

		/// <summary>
		/// _s the error.
		/// </summary>
		internal void _Error()
		{
			this.m_eStat = STATE.SOCKET_ERROR;
		}

		/// <summary>
		/// Start this update.
		/// </summary>
		internal void _Start()
		{
			this.m_eStat = STATE.START;
		}

		/// <summary>
		/// Fixeds the update.
		/// </summary>
		void FixedUpdate()
		{
			switch(this.m_eStat)
			{
			case STATE.START:
				if(this.m_cOnOpen != null )
				{
					this.m_cOnOpen();
				}
				this.m_cOnOpen = null;
				this.m_eStat = STATE.RUNING;
				break;
			case STATE.SOCKET_ERROR:
				if(this.m_cOnSocketError != null )
				{
					this.m_cOnSocketError();
				}
				this.m_eStat = STATE.RUNING;
				break;
			case STATE.RUNING:
				if(this.m_cUpdate != null )
				{
					this.m_cUpdate();
				}
				break;
			case STATE.CLOSE:
				if(this.m_cOnClose != null )
				{
					this.m_cOnClose();
				}
				this.m_cOnClose = null;
				this.m_eStat = STATE.NONE;
				GameObject.Destroy(gameObject);
				break;
			}
		}
	}
}

