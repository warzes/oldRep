using System;

namespace Shared.Network.Crypto
{
	public sealed class CoreCrypto
	{
		public bool ForServer { get; private set; }
		public uint Seed { get; private set; }

		private readonly CoreCipherEngine _cipher;
		private readonly CoreAesEngine _aesEngine;

		private static readonly Random _rnd = new Random();

		/// <summary>
		/// Initializes a new instance of Crypto with a random seed
		/// </summary>
		/// <param name="forServer">True if this instance will be responsible for packets
		/// sent by the server</param>
		public CoreCrypto(bool forServer) : this(GetRandomSeed(), forServer)
		{

		}

		/// <summary>
		/// Initializes a new instance of Crypto with the specified seed
		/// </summary>
		/// <param name="forServer">True if this instance will be responsible for packets
		/// sent by the server</param>
		public CoreCrypto(uint seed, bool forServer)
		{
			this.Seed = seed;
			this.ForServer = forServer;

			var keyGen = new CoreKeystreamGenerator(seed);
			_aesEngine = new CoreAesEngine(forServer, keyGen.AesKey);
			_cipher = new CoreCipherEngine(keyGen);
		}

		/// <summary>
		/// Returns a random seed
		/// </summary>
		private static uint GetRandomSeed()
		{
			return (uint)_rnd.Next(int.MinValue, int.MaxValue);
		}

		/// <summary>
		/// Applies the appropriate transformation to a packet that travels from the server to client
		/// </summary>
		public void FromServer(byte[] packet, int offset, int count)
		{
			_aesEngine.ProcessPacket(packet, offset, count);
		}

		/// <summary>
		/// Applies the appropriate transformation to a packet that travels from the server to client
		/// 
		/// This version uses the defaults for a standard packet
		/// </summary>
		public void FromServer(byte[] packet)
		{
			FromServer(packet, 6, packet.Length-6);
		}

		/// <summary>
		/// Applies the appropriate transformation to a packet that travels from the client to server.
		/// </summary>
		public void FromClient(byte[] packet, int offset, int count)
		{
			_cipher.ProcessPacket(packet, offset, count);
		}

		/// <summary>
		/// Applies the appropriate transformation to a packet that travels from the client to server.
		/// 
		/// This version uses the defaults for a standard packet
		/// </summary>
		public void FromClient(byte[] packet)
		{
			FromClient(packet, 6, packet.Length);
		}
	}
}