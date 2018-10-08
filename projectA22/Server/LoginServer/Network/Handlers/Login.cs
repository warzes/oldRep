using System;
using System.Text;
using Login.Database;
using Login.Util;
using Shared.Database;
using Core;
using Core.Const;
using Shared.Network;
using Shared.Util;
using System.Collections.Generic;
using Core.Network;
using Data;
using System.Threading;

namespace Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{
		/// <summary>
		/// First actual packet from the client, includes identification hash".
		/// </summary>
		[PacketHandler(Op.ClientIdent)]
		public void ClientIdent(LoginClient client, Packet packet)
		{
			var unkByte = packet.GetByte();
			client.Ident = packet.GetString();
			Send.CheckIdentR(client, true);
		}
		
		/// <summary>
		/// Login packet
		/// </summary>
		[PacketHandler(Op.Login)]
		public void Login(LoginClient client, Packet packet)
		{
			var loginType = (LoginType)packet.GetByte();
			var accountId = packet.GetString();
			var passbin = packet.GetBin();
			var password = Encoding.UTF8.GetString(passbin);
			// Upgrade MD5 to SHA1 (used by newer clients)
			if (password.Length == 32) // MD5
				password = Password.MD5ToSHA256(password);				

			// Create new account
			if (LoginServer.Instance.Conf.Login.NewAccounts && (accountId.StartsWith("new//") || accountId.StartsWith("new__")))
			{
				accountId = accountId.Remove(0, 5);

				if (!LoginServer.Instance.Database.AccountExists(accountId) && password != "")
				{
					LoginServer.Instance.Database.CreateAccount(accountId, password);
					Log.Info("New account '{0}' was created.", accountId);
				}
			}

			var machineId = packet.GetBin();
			var localClientIP = packet.GetString();

			// Get account
			var account = LoginServer.Instance.Database.GetAccount(accountId);
			if (account == null)
			{
				Send.LoginR_Fail(client, LoginResult.IdOrPassIncorrect);
				return;
			}

			// Check bans
			if (account.BannedExpiration > DateTime.Now)
			{
				Send.LoginR_Msg(client, ("You've been banned till {0}.\nReason: {1}"), account.BannedExpiration, account.BannedReason);
				return;
			}

			var sessionKey = 0L;
			// Check password/session
			if (!Password.Check(password, account.Password) && account.SessionKey != sessionKey)
			{
				Send.LoginR_Fail(client, LoginResult.IdOrPassIncorrect);
				return;
			}

			// Request logout
			// Wait till after the password, to prevent abuse.
			if (loginType == LoginType.RequestDisconnect)
				LoginServer.Instance.RequestDisconnect(account.Name);

			// Check login status
			if (LoginServer.Instance.Database.AccountIsLoggedIn(account.Name))
			{
				Send.LoginR_Fail(client, LoginResult.AlreadyLoggedIn);
				return;
			}

			account.SessionKey = LoginServer.Instance.Database.CreateSession(account.Name);

			// Update account
			account.LastLogin = DateTime.Now;
			LoginServer.Instance.Database.UpdateAccount(account);
			LoginServer.Instance.Database.SetAccountLoggedIn(account.Name, true);

			account.Characters = LoginServer.Instance.Database.GetCharacters(account.Name);

			// Success
			Send.LoginR(client, account, account.SessionKey, LoginServer.Instance.ServerList.List);

			client.Account = account;
			client.State = ClientState.LoggedIn;

			Log.Info("User '{0}' logged in.", account.Name);
		}
	}
}