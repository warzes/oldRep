using System;
using System.Text;
using Core.Network;
using Core.Const;
using Server.Network;
using Server.Network.Sending;
using Server.Util;
using Server;
using Server.World.Entities;

namespace Server.Network.Handlers
{
	public partial class ServerHandlers : PacketHandlerManager
	{
		[PacketHandler(Op.ChannelLogin)]
		public void ChannelLogin(Client client, Packet packet)
		{
			// Check state
			//if (client.State != ClientState.LoggingIn)
			//	return;
			if (client.State != ClientState.LoggedIn)
				return;
			
			var accountId = packet.GetString();
			var sessionKey = packet.GetLong();
			var characterId = packet.GetLong();
					
			// Check account
			var account = ServerApp.Instance.Database.GetAccountWorld(accountId);
			if (account == null || account.SessionKey != sessionKey)
			{
				Log.Warning("Server handler: Invalid account ({0}) or session ({1}).", accountId, sessionKey);
				client.Kill();
				return;
			}

			// Check character
			var character = account.GetCharacterSafe(characterId) as Creature;
				
			client.Account = account;
			client.Controlling = character;
			client.Creatures.Add(character.EntityId, character);
			character.Client = client;

			client.State = ClientState.LoggedInWorld;

			// Update online status
			ServerApp.Instance.Database.SetAccountLoggedIn(account.Id, true);

			var playerCreature = character as PlayerCreature;
			if (playerCreature != null)
				ServerApp.Instance.Database.UpdateOnlineStatus(playerCreature.CreatureId, true);
			ServerApp.Instance.Database.UpdateOnlineStatus((character as PlayerCreature).CreatureId, true);

			// Response
			Send.ChannelLoginR(client, character.EntityId);

			// Log into world
			// Fallback for invalid region ids, like 0, dynamics, and dungeons.
			if (character.RegionId == 0 || Math2.Between(character.RegionId, 35000, 40000) || Math2.Between(character.RegionId, 10000, 11000))
				character.SetLocation(1, 12800, 38100);

			character.Warp(character.GetLocation());
		}
	}
}
