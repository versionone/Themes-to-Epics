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
			if (args.Length > 0)
				Scope = args[0];
			if (args.Length > 1)
				Url = args[1];
			if (args.Length > 2)
				Username = args[2];
			if (args.Length > 3)
				Password = args[3];
			return this;
		}

		public Options Validate()
		{
			return this;
		}

		public string Scope { get; set; }
		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}