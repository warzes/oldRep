namespace Shared.Database
{
	/// <summary>
	/// Base class for holding references to all guilds and synchronizing them with the database.
	/// </summary>
	public abstract class AbstractGuildManager
	{
		protected object _syncLock = new object();
	}
}
