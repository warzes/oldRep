using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Core.Network;
using Server.Util;
using Server.Network.Crypto;
using Server.Database.CoreData;
using Server.World.Entities;

namespace Server.Network
{
	public enum ClientState { BeingChecked, LoggingIn, LoggedIn, LoggedInWorld, Dead }

	public class Client
	{
		private const int BufferDefaultSize = 2048;

		public Socket Socket { get; set; }
		public byte[] Buffer { get; set; }

		public ClientState State { get; set; }
		public ServerCrypto Crypto { get; set; }
		
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

		public Account Account { get; set; }
		public Creature Controlling { get; set; }
		public Dictionary<long, Creature> Creatures { get; protected set; }

		public Client()
		{
			this.Buffer = new byte[BufferDefaultSize];
			this.Crypto = new ServerCrypto(0x0, true);
			this.Creatures = new Dictionary<long, Creature>();
		}

		public void Send(byte[] buffer)
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

		public virtual void Send(Packet packet)
		{
			//Log.Debug("S: " + packet);
			this.Send(this.BuildPacket(packet));
		}

		public void EncodeBuffer(byte[] buffer)
		{
			// sample
			//buffer[5] = 0x03;
		}

		public void DecodeBuffer(byte[] buffer)
		{
			//this.Crypto.FromClient(buffer);
		}

		byte[] BuildPacket(Packet packet)
		{
			var buildPack = new byte[6 + packet.GetSize() + 4]; // header + packet + checksum
			buildPack[0] = 0x80;
			System.Buffer.BlockCopy(BitConverter.GetBytes(buildPack.Length), 0, buildPack, 1, sizeof(int));
			packet.Build(ref buildPack, 6);

			return buildPack;
		}

		/// <summary>
		/// Kills client connection.
		/// </summary>
		public void Kill()
		{
			if (this.State != ClientState.Dead)
			{
				try { this.Socket.Shutdown(SocketShutdown.Both); }
				catch { }

				try { this.Socket.Close(); }
				catch { }

				try	{ this.CleanUp(); }
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

		public void CleanUp()
		{
			if (this.Account != null) 
				ServerApp.Instance.Database.SetAccountLoggedIn(this.Account.Name, false);
		}
	}
}