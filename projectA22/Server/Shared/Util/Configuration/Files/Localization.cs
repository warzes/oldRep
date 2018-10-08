using System.Globalization;
using System.Threading;

namespace Shared.Util.Configuration.Files
{
	/// <summary>
	/// Represents localization.conf
	/// </summary>
	public class LocalizationConfFile : ConfFile
	{
		public string Language { get; protected set; }
		public string Culture { get; protected set; }
		public string CultureUi { get; protected set; }

		public void Load()
		{
			this.Require("system/conf/localization.conf");

			this.Language = this.GetString("language", "en-US");
			this.Culture = this.GetString("culture", "en-US");
			this.CultureUi = this.GetString("culture_ui", "en-US");

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(this.Culture);
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(this.CultureUi);
			Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
		}
	}
}