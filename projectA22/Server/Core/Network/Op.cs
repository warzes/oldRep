using System.Reflection;

namespace Core.Network
{
	/// <summary>
	/// All Op codes
	/// </summary>
	public static class Op
	{
		// Login Server
		// ------------------------------------------------------------------
		public const int ClientIdent = 0x0FD1020A;
		public const int ClientIdentR = 0x1F;
		public const int Login = 0x0FD12002;
		public const int LoginR = 0x23;
		public const int ChannelStatus = 0x26;
		public const int ChannelInfoRequest = 0x2F;
		public const int ChannelInfoRequestR = 0x30;		
		public const int DisconnectInform = 0x4D;

		// Messenger Server
		// ------------------------------------------------------------------
		public static class Msgr
		{			
		}

		// Internal communication
		// ------------------------------------------------------------------
		public static class Internal
		{
			public const int ServerIdentify = 0x42420001;
			public const int ServerIdentifyR = 0x42420002;
			public const int ChannelStatus = 0x42420101;
			public const int BroadcastNotice = 0x42420201;
			public const int ChannelShutdown = 0x42420301;
			public const int RequestDisconnect = 0x42420401;
		}

		/// <summary>
		/// Returns name of op code, if it's defined.
		/// </summary>
		public static string GetName(int op)
		{
			// Login/Channel
			foreach (var field in typeof(Op).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				if ((int)field.GetValue(null) == op)
					return field.Name;
			}

			// Msgr
			foreach (var field in typeof(Op.Msgr).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				if ((int)field.GetValue(null) == op)
					return "Msgr." + field.Name;
			}

			return "?";
		}
	}
}