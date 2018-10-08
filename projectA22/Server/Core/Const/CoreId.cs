namespace Core.Const
{
	public static class CoreId
	{
		/// <summary>
		/// Used as packet id by the login server, since there's no actual target creature yet.
		/// </summary>
		public const long Login = 0x1000000000000010;

		/// <summary>
		/// Used by the channel server when there's no actual target creature, like during login/out.
		/// </summary>
		public const long Channel = 0x1000000000000001;

		/// <summary>
		/// Start of the dynamic regions id range
		/// </summary>
		public const int DynamicRegions = 35001;
	}
}