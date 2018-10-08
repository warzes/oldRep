namespace Core.Const
{
	public enum ChannelState
	{
		/// <summary>
		/// Server is offline for maintenance.
		/// </summary>
		Maintenance = 0,

		/// <summary>
		/// Server is online and stress is 0~39%.
		/// </summary>
		Normal = 1,

		/// <summary>
		/// Server is online and stress is 40~69%.
		/// </summary>
		Busy = 2,

		/// <summary>
		/// Server is online and stress is 70~94%.
		/// </summary>
		Full = 3,

		/// <summary>
		/// Server is online and stress is 95~100%.
		/// </summary>
		/// <remarks>
		/// In this state, the client won't allow the player to move to the channel.
		/// </remarks>
		Bursting = 4,
		
		/// <summary>
		/// Any other value is interpreted as Error
		/// </summary>
		Error = 5
	}
}