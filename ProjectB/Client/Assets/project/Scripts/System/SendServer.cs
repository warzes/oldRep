using Core.Network;

public class SendServer
{
	public static void ClientIdent()
	{
		var packet = new Packet(Op.ClientIdent, 0);
		packet.PutByte(true);
		// TODO: данные для проверки клиента?
		Connection.Client.Send(packet);
	}

	public static void Login(string username, byte[] sbHash)
	{
		Packet packet = new Packet(Op.Login, 0);
		packet.PutString(username);
		packet.PutBin(sbHash);
		packet.PutBin();	// machine Id
		packet.PutString(Connection.Client.GetLocalIp());

		Connection.Client.Send(packet);
	}

	public static void ChannelLogin(long entityId)
	{
		var packet = new Packet(Op.ChannelLogin, 0x200000000000000F);
		packet.PutString(Connection.AccountName);
		packet.PutLong(Connection.SessionKey);
		packet.PutLong(entityId);
		Connection.Client.Send(packet);
	}
}
