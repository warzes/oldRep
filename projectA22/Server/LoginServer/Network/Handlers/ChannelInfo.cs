using Login.Database;
using Core.Network;
using Shared.Network;

namespace Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{
		/// <summary>
		/// Request for connection information for the channel.
		/// </summary>
		/// <remarks>
		/// Sent after disconnect info, which makes the client stuck if the connection to the channel fails.
		/// </remarks>
		[PacketHandler(Op.ChannelInfoRequest)]
		public void ChannelInfoRequest(LoginClient client, Packet packet)
		{
			var serverName = packet.GetString();
			var channelName = packet.GetString();
			var rebirth = packet.GetBool();
			var characterId = packet.GetLong();

			// Check channel and character
			var channelInfo = LoginServer.Instance.ServerList.GetChannel(serverName, channelName);
			var character = client.Account.GetCharacter(characterId);

			if (channelInfo == null || character == null)
			{
				Send.ChannelInfoRequestR_Fail(client);
				return;
			}

			if (rebirth)
				LoginServer.Instance.Database.UninitializeCreature(character.CreatureId);

			// Success
			Send.ChannelInfoRequestR(client, channelInfo, characterId);
		}
	}
}