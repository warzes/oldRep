using System;
using System.Text;
using Core.Network;
using Core.Const;
using Server.Network;
using Server.Network.Sending;
using Server.Util;
using Server;

namespace Server.Network.Handlers
{
	public partial class ServerHandlers : PacketHandlerManager
	{
		[PacketHandler(Op.ClientIdent)]
		public void ClientIdent(Client client, Packet packet)
		{
			var unkByte = packet.GetByte();
			Send.CheckIdentR(client, true);
		}

		[PacketHandler(Op.Login)]
		public void Login(Client client, Packet packet)
		{
			var accountId = packet.GetString();
			var password = "";
			var passbin = packet.GetBin();
			password = Encoding.UTF8.GetString(passbin);
			if (password.Length == 32) // MD5
				password = Password.MD5ToSHA256(password);
			
			// Create new account
			if (ServerApp.Instance.Conf.Server.NewAccounts && (accountId.StartsWith("new//") || accountId.StartsWith("new__")))
			{
				accountId = accountId.Remove(0, 5);

				if (!ServerApp.Instance.Database.AccountExists(accountId) && password != "")
				{
					ServerApp.Instance.Database.CreateAccount(accountId, password);
					Log.Info("New account '{0}' was created.", accountId);
				}
			}

			var machineId = packet.GetBin();
			var localClientIP = packet.GetString();

			// Get account
			var account = ServerApp.Instance.Database.GetAccount(accountId);
			if (account == null)
			{
				Send.LoginR_Fail(client, LoginResult.IdOrPassIncorrect);
				return;
			}

			var sessionKey = 0L;
			// Check password/session
			if (!Password.Check(password, account.Password) && account.SessionKey != sessionKey)
			{
				Send.LoginR_Fail(client, LoginResult.IdOrPassIncorrect);
				return;
			}
			
			// Check login status
			if (ServerApp.Instance.Database.AccountIsLoggedIn(account.Name))
			{
				Send.LoginR_Fail(client, LoginResult.AlreadyLoggedIn);
				return;
			}

			account.SessionKey = ServerApp.Instance.Database.CreateSession(account.Name);
			
			// Update account
			account.LastLogin = DateTime.Now;
			ServerApp.Instance.Database.UpdateAccount(account);
			ServerApp.Instance.Database.SetAccountLoggedIn(account.Name, true);

			account.Characters = ServerApp.Instance.Database.GetCharacters(account.Name);
						
			// Success
			Send.LoginR(client, account, account.SessionKey);

			client.Account = account;
			client.State = ClientState.LoggedIn;

			Log.Info("User '{0}' logged in.", account.Name);
		}
	}
}