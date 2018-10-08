using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Core.Network;
using Server.Util;

namespace Server.Network
{
	public class Server
	{
		public delegate void ClientConnectionEventHandler(Client client);

		private Socket _socket;
		public List<Client> Clients { get; set; }
		public PacketHandlerManager Handlers { get; set; }

		public event ClientConnectionEventHandler ClientConnected;
		public event ClientConnectionEventHandler ClientDisconnected;

		public Server()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.NoDelay = true;
			this.Clients = new List<Client>();
		}

		public void Start(int port)
		{
			this.Start(new IPEndPoint(IPAddress.Any, port));
		}

		public void Start(string host, int port)
		{
			this.Start(new IPEndPoint(IPAddress.Parse(host), port));
		}

		private void Start(IPEndPoint endPoint)
		{
			if (this.Handlers == null)
			{
				Log.Error("No packet handler manager set, start canceled.");
				return;
			}

			try
			{
				_socket.Bind(endPoint);
				_socket.Listen(10);

				_socket.BeginAccept(this.OnAccept, _socket);

				Log.Status("Server ready, listening on {0}.", _socket.LocalEndPoint);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Unable to set up socket; perhaps you're already running a server?");
				CliUtil.Exit(1);
			}
		}

		public void Stop()
		{
			try
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Close();
			}
			catch
			{ }
		}

		private void OnAccept(IAsyncResult result)
		{
			var client = new Client();

			try
			{
				client.Socket = (result.AsyncState as Socket).EndAccept(result);

				// We don't need this here, since it's inherited from the parent
				// client.Socket.NoDelay = true;

				client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, this.OnReceive, client);

				this.AddClient(client);
				Log.Info("Connection established from '{0}.", client.Address);

				this.OnClientConnected(client);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "While accepting connection.");
			}
			finally
			{
				_socket.BeginAccept(this.OnAccept, _socket);
			}
		}

		public void AddReceivingClient(Client client)
		{
			client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, this.OnReceive, client);
		}

		/// <summary>
		/// Handles sending packets, obviously.
		/// </summary>
		protected void OnReceive(IAsyncResult result)
		{
			var client = result.AsyncState as Client;

			try
			{
				int bytesReceived = client.Socket.EndReceive(result);
				int ptr = 0;

				if (bytesReceived == 0)
				{
					Log.Info("Connection closed from '{0}.", client.Address);
					this.KillAndRemoveClient(client);
					this.OnClientDisconnected(client);
					return;
				}

				// Handle all received bytes
				while (bytesReceived > 0)
				{
					// Length of new packet
					int length = this.GetPacketLength(client.Buffer, ptr);

					// Shouldn't actually happen...
					if (length > client.Buffer.Length)
						throw new Exception(string.Format("Buffer too small to receive full packet ({0}).", length));

					// Read whole packet and ...
					var buffer = new byte[length];
					Buffer.BlockCopy(client.Buffer, ptr, buffer, 0, length);
					bytesReceived -= length;
					ptr += length;

					// Handle it
					this.HandleBuffer(client, buffer);
				}

				// Stop if client was killed while handling.
				if (client.State == ClientState.Dead)
				{
					Log.Info("Killed connection from '{0}'.", client.Address);
					this.RemoveClient(client);
					this.OnClientDisconnected(client);
					return;
				}

				// Round $round+1, receive!
				client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, this.OnReceive, client);
			}
			catch (SocketException)
			{
				Log.Info("Connection lost from '{0}'.", client.Address);
				this.KillAndRemoveClient(client);
				this.OnClientDisconnected(client);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "While receiving data from '{0}'.", client.Address);
				this.KillAndRemoveClient(client);
				this.OnClientDisconnected(client);
			}
		}

		protected void KillAndRemoveClient(Client client)
		{
			client.Kill();
			this.RemoveClient(client);
		}

		protected void AddClient(Client client)
		{
			lock (this.Clients)
			{
				this.Clients.Add(client);
				//Log.Status("Connected clients: {0}", _clients.Count);
			}
		}

		protected void RemoveClient(Client client)
		{
			lock (this.Clients)
			{
				this.Clients.Remove(client);
				//Log.Status("Connected clients: {0}", _clients.Count);
			}
		}

		protected void OnClientConnected(Client client)
		{
			// Send seed
			client.Socket.Send(BitConverter.GetBytes(client.Crypto.Seed));

			if (this.ClientConnected != null)
				this.ClientConnected(client);
		}

		protected void OnClientDisconnected(Client client)
		{
			if (this.ClientDisconnected != null)
				this.ClientDisconnected(client);
		}

		protected int GetPacketLength(byte[] buffer, int ptr)
		{
			return
				(buffer[ptr + 1] << 0) +
				(buffer[ptr + 2] << 8) +
				(buffer[ptr + 3] << 16) +
				(buffer[ptr + 4] << 24);
		}

		protected void HandleBuffer(Client client, byte[] buffer)
		{			
			var length = buffer.Length;
			// Cut 4 bytes at the end
			Array.Resize(ref buffer, length -= 4);
			// Write new length into the buffer.
			BitConverter.GetBytes(length).CopyTo(buffer, 1);

			// Decrypt packet if crypt flag isn't 3.
			//if (buffer[5] != 0x03)
			//	client.DecodeBuffer(buffer);

			//Log.Debug("in:  " + BitConverter.ToString(buffer));

			// Flag 1 is a heartbeat, acknowledge and send back.
			// TODO: ???
			//if (buffer[5] == 0x01)
			//{
			//	BitConverter.GetBytes(BitConverter.ToUInt32(buffer, 6) ^ 0x98BADCFE).CopyTo(buffer, 6);
			//	client.Socket.Send(buffer);
			//}
			//else
			{
				// First packet, skip challenge and send success msg.
				if (client.State == ClientState.BeingChecked)
				{
					client.Send(new byte[] { 0x80, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 });
					client.State = ClientState.LoggingIn;
				}
				// Actual packets
				else
				{
					// Start reading after the protocol header
					var packet = new Packet(buffer, 6);
					//Log.Debug(packet);

					try
					{
						this.Handlers.Handle(client, packet);
					}
					catch (Exception ex)
					{
						Log.Exception(ex, "There has been a problem while handling '{0:X4}', '{1}'.", packet.Op, Op.GetName(packet.Op));
					}
				}
			}
		}
	}
}