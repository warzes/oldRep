﻿using System;
using System.Net.Sockets;
using Shared.Network.Crypto;
using Shared.Util;
using Core.Network;

namespace Shared.Network
{
	public enum ClientState { BeingChecked, LoggingIn, LoggedIn, Dead }

	/// <summary>
	/// Base client, for specialized client classes in the servers.
	/// </summary>
	public abstract class BaseClient
	{
		// Largest known packet is composing on R1, up to ~3700 bytes.
		private const int BufferDefaultSize = 4096;

		public Socket Socket { get; set; }
		public byte[] Buffer { get; set; }
		public ClientState State { get; set; }
		public CoreCrypto Crypto { get; set; }

		private string _address;
		public string Address
		{
			get
			{
				if (_address == null)
				{
					try
					{
						_address = this.Socket.RemoteEndPoint.ToString();
					}
					catch
					{
						_address = "?";
					}
				}

				return _address;
			}
		}

		protected BaseClient()
		{
			this.Buffer = new byte[BufferDefaultSize];
			this.Crypto = new CoreCrypto(0x0, true); // 0x41757261
		}

		/// <summary>
		/// Sends buffer
		/// </summary>
		public virtual void Send(byte[] buffer)
		{
			if (this.State == ClientState.Dead)
				return;

			this.EncodeBuffer(buffer);

			//Log.Debug("out: " + BitConverter.ToString(buffer));

			try
			{
				this.Socket.Send(buffer);
			}
			catch (Exception ex)
			{
				Log.Error("Unable to send packet to '{0}'. ({1})", this.Address, ex.Message);
			}
		}

		/// <summary>
		/// Builds buffer from packet and sends it.
		/// </summary>
		public virtual void Send(Packet packet)
		{
			// Don't log internal packets
			//if (packet.Op < Op.Internal.ServerIdentify)
			//    Log.Debug("S: " + packet);

			this.Send(this.BuildPacket(packet));
		}

		/// <summary>
		/// Encodes buffer.
		/// </summary>
		protected abstract void EncodeBuffer(byte[] buffer);

		public abstract void DecodeBuffer(byte[] buffer);

		/// <summary>
		/// Builds packet, appending the overall header and checksum.
		/// </summary>
		protected abstract byte[] BuildPacket(Packet packet);

		/// <summary>
		/// Kills client connection.
		/// </summary>
		public virtual void Kill()
		{
			if (this.State != ClientState.Dead)
			{
				try { this.Socket.Shutdown(SocketShutdown.Both); }
				catch { }

				try { this.Socket.Close(); }
				catch { }

				try
				{
					// Naturally, we have to clean up after killing somebody.
					this.CleanUp();
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "While cleaning up after client.");
				}

				this.State = ClientState.Dead;
			}
			else
			{
				Log.Warning("Client got killed multiple times." + Environment.NewLine + Environment.StackTrace);
			}
		}

		/// <summary>
		/// Takes care of client's remains (saving chars, etc)
		/// </summary>
		public virtual void CleanUp()
		{

		}
	}
}