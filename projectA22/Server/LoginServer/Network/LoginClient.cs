using Login.Database;
using Shared.Network;

namespace Login.Network
{
	public class LoginClient : DefaultClient
	{
		public string Ident { get; set; }
		public Account Account { get; set; }

		public override void CleanUp()
		{
			if (this.Account != null)
				LoginServer.Instance.Database.SetAccountLoggedIn(this.Account.Name, false);
		}
	}
}