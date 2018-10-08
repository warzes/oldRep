using System;
using System.Collections.Generic;
using System.Linq;
using Core.Network;
using Server.Util;

namespace Server.Network
{
	/// <summary>
	/// Methods having this attribute are registered as packet handlers, for the ops.
	/// </summary>
	public class PacketHandlerAttribute : Attribute
	{
		public int[] Ops { get; protected set; }

		public PacketHandlerAttribute(params int[] ops)
		{
			this.Ops = ops;
		}
	}

	/// <summary>
	/// Packet handler manager base class.
	/// </summary>
	// TODO: может не абстрактный?
	public abstract class PacketHandlerManager
	{
		public delegate void PacketHandlerFunc(Client client, Packet packet);

		private Dictionary<int, PacketHandlerFunc> _handlers;

		protected PacketHandlerManager()
		{
			_handlers = new Dictionary<int, PacketHandlerFunc>();
		}

		/// <summary>
		/// Adds and/or overwrites handler.
		/// </summary>
		public void Add(int op, PacketHandlerFunc handler)
		{
			_handlers[op] = handler;
		}

		/// <summary>
		/// Adds all methods with a Handler attribute.
		/// </summary>
		public void AutoLoad()
		{
			foreach (var method in this.GetType().GetMethods())
			{
				foreach (PacketHandlerAttribute attr in method.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
				{
					var del = (PacketHandlerFunc)Delegate.CreateDelegate(typeof(PacketHandlerFunc), this, method);
					foreach (var op in attr.Ops)
						this.Add(op, del);
				}
			}
		}

		/// <summary>
		/// Runs handler for packet's op, or logs it as unimplemented.
		/// </summary>
		public void Handle(Client client, Packet packet)
		{
			//Log.Debug("R: " + packet);
			PacketHandlerFunc handler;
			if (!_handlers.TryGetValue(packet.Op, out handler))
			{
				this.UnknownPacket(client, packet);
				return;
			}

			try
			{
				handler(client, packet);
			}
			catch (PacketElementTypeException ex)
			{
				Log.Error(
					"PacketElementTypeException: " + ex.Message + Environment.NewLine +
					ex.StackTrace + Environment.NewLine +
					"Packet: " + Environment.NewLine +
					packet.ToString()
				);
			}			
		}


		public virtual void UnknownPacket(Client client, Packet packet)
		{
			Log.Unimplemented("PacketHandlerManager: Handler for '{0:X4}', '{1}'.", packet.Op, Op.GetName(packet.Op));
			Log.Debug(packet);
		}
	}
}