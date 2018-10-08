using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Core.Const;
using Core.Network;
using Core;


public enum CharacterSelectState
{
	Waiting,
	Connecting,
	Login,
	LoggedIn,
}

public class CharacterSelectList : MonoBehaviour
{
	public Transform ButtonsParent;
	public GameObject ButtonReference;
	public Button BtnStart;

	[HideInInspector]
	public CharacterSelectState State = CharacterSelectState.Waiting;

	private CharacterInfo[] characters;
	private CharacterInfo selectedCharacter;
	private CharacterSelectState prevState = CharacterSelectState.Waiting;
	private MsgBox stateInfo;

	void Start()
	{
		characters = Connection.Characters.OrderBy(a => a.EntityId).ToArray();
		foreach (var character in characters)
		{
			var entityId = character.EntityId;
			var name = string.Format("{0}", character.Name);

			var buttonObj = GameObject.Instantiate(ButtonReference);
			buttonObj.transform.SetParent(ButtonsParent);

			var text = buttonObj.GetComponentInChildren<Text>();
			text.text = name;

			var disable = buttonObj.transform.FindChild("Img" + ("Character"));
			disable.gameObject.SetActive(false);

			var button = buttonObj.GetComponent<Button>();
			button.onClick.AddListener(() => { OnCharacterSelected(entityId); });
		}

		BtnStart.interactable = false;
		BtnStart.onClick.AddListener(BtnStart_OnClick);
		
		//TxtSelectedChar.gameObject.SetActive(characters.Length != 0);
		//if (characters.Length != 0)
		//	OnCharacterSelected(characters[0].EntityId);
	}

	void Update()
	{
		var connecting = false;
		if (prevState != State && State == CharacterSelectState.Connecting)
		{
			if (stateInfo != null)
				stateInfo.Close();

			stateInfo = MsgBox.Show("Connecting...", MsgBoxButtons.None);
			connecting = true;
		}

		prevState = State;

		if (connecting) return;

		if (State != CharacterSelectState.Waiting && Connection.Client.State == ConnectionState.Disconnected)
		{
			State = CharacterSelectState.Waiting;

			if (stateInfo != null)
				stateInfo.Close();

			MsgBox.Show("Failed to connect.");
		}
		else if (State == CharacterSelectState.Connecting && Connection.Client.State == ConnectionState.Connected)
		{
			SendServer.ChannelLogin(selectedCharacter.EntityId);
			State = CharacterSelectState.Login;
		}
	}

	private void OnCharacterSelected(long entityId)
	{
		//selectedCharacter = characters.FirstOrDefault(a => a.EntityId == entityId);
		//if (selectedCharacter == null)
		//{
		//	Debug.LogErrorFormat("Missing character '{0}'.", entityId);
		//	return;
		//}

		//UpdateChannels(selectedCharacter.Server);
		//SelChannel.interactable = true;

		//var server = selectedCharacter.Server;
		//var name = selectedCharacter.Name;
		//var color = CoreMath.GetNameColor(name).ToString("X6");

		//TxtSelectedChar.text = string.Format("<size=20>{0}</size>\n<color=#{2}>{1}</color>", server, name, color);
		//Connection.SelectedCharacter = selectedCharacter;
	}
	
	private void BtnStart_OnClick()
	{
		if (selectedCharacter == null)
		{
			Debug.LogError("Selected character is null.");
			return;
		}

		//SendLoginServer.ChannelInfoRequest(selectedCharacter.Server, selectedChannel.Name, selectedCharacter.EntityId);

		////State = CharacterSelectState.Login;
	}
}
