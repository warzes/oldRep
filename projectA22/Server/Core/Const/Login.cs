namespace Core.Const
{
	public enum LoginType
	{
		/// <summary>
		/// Used to request disconnect when you're already logged in.
		/// </summary>
		RequestDisconnect = 1,
		/// <summary>
		/// Default, hashed password
		/// </summary>
		Normal = 2,
	}

	public enum LoginResult
	{
		Fail = 0,
		Success = 1,
		Empty = 2,
		IdOrPassIncorrect = 3,
		TooManyConnections = 6,
		AlreadyLoggedIn = 7,
		Message = 51,
		Banned = 101,
	}

	public enum LoginResultMessage
	{
		Custom = 18,
	}
	
	public enum DeletionFlag { Normal, Recover, Ready, Delete }
}