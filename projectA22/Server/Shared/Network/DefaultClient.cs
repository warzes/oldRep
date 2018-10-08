using Core.Network;
using System;

namespace Shared.Network
{
	/// <summary>
	/// Normal client (Login, Channel).
	/// </summary>
	public class DefaultClient : BaseClient
	{
		protected override void EncodeBuffer(byte[] buffer)
		{
			// Set raw flag
			buffer[5] = 0x03;
		}

		public override void DecodeBuffer(byte[] buffer)
		{
			this.Crypto.FromClient(buffer);
		}

		protected override byte[] BuildPacket(Packet packet)
		{
			var result = new byte[6 + packet.GetSize() + 4]; // header + packet + checksum
			result[0] = 0x88;
			System.Buffer.BlockCopy(BitConverter.GetBytes(result.Length), 0, result, 1, sizeof(int));
			packet.Build(ref result, 6);

			return result;
		}
	}
}