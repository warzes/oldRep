using UnityEngine;
using System.Collections;

public class DisconnectOnClose : MonoBehaviour
{
	void OnApplicationQuit()
	{
		Connection.Client.Disconnect();
	}
}