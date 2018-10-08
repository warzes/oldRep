using System.Reflection;

namespace Core.Network
{
	public static class Op
	{
		// Login
		// ------------------------------------------------------------------
		public const int ClientIdent = 100;
		public const int ClientIdentR = 101;
		public const int Login = 102;
		public const int LoginR = 103;


		// Channel
		// ------------------------------------------------------------------
		public const int ChannelLogin = 300;
		public const int ChannelLoginR = 301;

public const int Chat = 0x526C; // TODO
public const int EnterRegion = 0x6597; // TODO
public const int CharacterLock = 0x701E; // TODO
public const int CharacterUnlock = 0x701F; // TODO
public const int EnterDynamicRegion = 0x9571;  // TODO

		// Messenger
		// ------------------------------------------------------------------
		public static class Msgr
		{
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