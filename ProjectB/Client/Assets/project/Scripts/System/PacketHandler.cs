using Core.Const;
using Core.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PacketHandler : MonoBehaviour
{
	private MsgBox regionLoadFailMsg;

	private delegate void PacketHandlerFunc(Packet packet);

	private class PacketHandlerAttribute : Attribute
	{
		public int[] Ops { get; protected set; }

		public PacketHandlerAttribute(params int[] ops)
		{
			this.Ops = ops;
		}
	}

	private Dictionary<int, PacketHandlerFunc> _handlers = new Dictionary<int, PacketHandlerFunc>();

	void Start()
	{
		foreach (var method in this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
		{
			foreach (PacketHandlerAttribute attr in method.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
			{
				var del = (PacketHandlerFunc)Delegate.CreateDelegate(typeof(PacketHandlerFunc), this, method);
				foreach (var op in attr.Ops)
					_handlers[op] = del;
			}
		}
	}

	void Update()
	{
		HandlePackets();

		if (regionLoadFailMsg != null && regionLoadFailMsg.Result != MsgBoxResult.Pending)
		{
			regionLoadFailMsg = null;
			Connection.Client.Disconnect();
			SceneManager.LoadScene("Login");
		}
	}

	private void HandlePackets()
	{
		if (Connection.Client.State != ConnectionState.Connected)
			return;

		var packets = Connection.Client.GetPacketsFromQueue();
		foreach (var packet in packets)
		{
			//Debug.Log(packet);

			PacketHandlerFunc handler;
			if (!_handlers.TryGetValue(packet.Op, out handler))
			{
				Debug.LogFormat("Unhandled packet: {0:X4} ({1})", packet.Op, Op.GetName(packet.Op));
				continue;
			}

			try
			{
				handler(packet);
			}
			catch (PacketElementTypeException ex)
			{
				Debug.LogException(ex);
				Debug.Log("Packet: " + packet);
			}
		}
	}

	private T GetComponentIn<T>(string gameObjectName) where T : MonoBehaviour
	{
		var gameObj = GameObject.Find(gameObjectName);
		if (gameObj == null)
		{
			Debug.LogError("GetComponentIn: " + gameObjectName + " not found.");
			return null;
		}

		var component = gameObj.GetComponent<T>();
		if (component == null)
		{
			Debug.LogError("GetComponentIn: " + typeof(T).Name + " component not found.");
			return null;
		}

		return component;
	}
}