using System.Collections.Specialized;

namespace VersionOne.Themes_to_Epics
{
	internal class Options
	{
		public Options Load(NameValueCollection settings)
		{
			if (settings["Url"] != null)
				Url = settings["Url"];
			if (settings["Username"] != null)
				Username = settings["Username"];
			if (settings["Password"] != null)
				Password = settings["Password"];
			return this;
		}

		public Options Load(string[] args)
		{
			return this;
		}

		public Options Validate()
		{
			return this;
		}

		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}