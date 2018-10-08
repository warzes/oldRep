using Login.Database;
using Core.Network;
using Shared.Network;
using Shared.Util;

namespace Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{
		/// <summary>
		/// Sent before going back to login screen or continuing to a channel, apparently related to the favorites list.
		/// </summary>
		[PacketHandler(Op.DisconnectInform)]
		public void Disconnect(LoginClient client, Packet packet)
		{
			var accountName = packet.GetString();
			var unkSessionKey = packet.GetLong();
			
			if (accountName != client.Account.Name) return;

			LoginServer.Instance.Database.UpdateAccount(client.Account);

			Log.Info("'{0}' is closing the connection.", accountName);
		}
	}
}