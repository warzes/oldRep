namespace Shared.Scripting.Scripts
{
	public interface IScript
	{
		bool Init();
	}

	public interface IAutoLoader
	{
		void AutoLoad();
	}
}
