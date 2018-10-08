using Core.Const;
using Core.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PacketHandler
{
#pragma warning disable 0168
	[PacketHandler(Op.ClientIdentR)]
	private void ClientIdentR(Packet packet)
	{
		var form = GetComponentIn<LoginForm>("FrmLogin");
		if (form == null || form.State != LoginState.Ident)
		{
			Debug.Log("Received ClientIdentR outside of login or in incorrect state.");
			return;
		}

		var username = form.TxtUsername.text;
		var password = form.TxtPassword.text;

		var md5 = MD5.Create();
		var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
		var sHash = BitConverter.ToString(hash).Replace("-", "");
		var sbHash = Encoding.UTF8.GetBytes(sHash);

		SendServer.Login(username, sbHash);
		form.State = LoginState.Login;
	}

	[PacketHandler(Op.LoginR)]
	private void LoginR(Packet packet)
	{
		var form = GetComponentIn<LoginForm>("FrmLogin");
		if (form == null || form.State != LoginState.Login)
		{
			Debug.Log("Received LoginR outside of login or in incorrect state.");
			return;
		}

		var result = (LoginResult)packet.GetByte();
		if (result != LoginResult.Success)
		{
			switch (result)
			{
				case LoginResult.Message:
					var unkLoginResultMessage = packet.GetInt();
					var message = packet.GetString();
					form.ResetForm(message);
					break;

				case LoginResult.IdOrPassIncorrect: form.ResetForm("The username or password is incorrect."); break;
				case LoginResult.AlreadyLoggedIn: form.ResetForm("This account is already logged in."); break;
				default: form.ResetForm("Login failed."); break;
			}
		}
		else
		{
			// Parse

			var accountName = packet.GetString();
			var sessionKey = packet.GetLong();
			// Account Info
			var lastLogin = packet.GetDateTime();
			var lastLogout = packet.GetDateTime();
			// Characters
			var characters = new List<CharacterInfo>();
			var characterCount = packet.GetShort();
			for (int i = 0; i < characterCount; ++i)
			{
				var entityId = packet.GetLong();
				var characterName = packet.GetString();
				
				var character = new CharacterInfo();
				character.EntityId = entityId;
				character.Name = characterName;
				characters.Add(character);
			}
			
			// Set
			Connection.AccountName = accountName;
			Connection.SessionKey = sessionKey;
			Connection.Characters.Clear();
			Connection.Characters.AddRange(characters);

			// Transition
			SceneManager.LoadScene("CharacterSelect");
			form.State = LoginState.LoggedIn;
		}
	}

	[PacketHandler(Op.ChannelLoginR)]
	private void ChannelLoginR(Packet packet)
	{
		var list = GetComponentIn<CharacterSelectList>("LstCharacters");
		if (list == null || list.State != CharacterSelectState.Login)
		{
			Debug.Log("Received ChannelLoginR outside of character selection or in incorrect state.");
			return;
		}

		var success = packet.GetBool();
		if (!success)
		{
			MsgBox.Show("No channel information received.");
			return;
		}

		var creatureEntityId = packet.GetLong();
		var now = packet.GetDateTime();

		list.State = CharacterSelectState.LoggedIn;
	}

#pragma warning restore 0168
}
