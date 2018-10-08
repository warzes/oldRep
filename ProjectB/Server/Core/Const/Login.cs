namespace Core.Const
{
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
}
