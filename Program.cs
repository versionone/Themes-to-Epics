using System;
using System.Configuration;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				Options options = new Options()
					.Load(ConfigurationManager.AppSettings)
					.Load(args)
					.Validate();
				Run(args);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.Message);
				return 2;
			}
			return 0;
		}

		private static void Run(string[] args)
		{
			V1Instance v1Instance = new V1Instance("http://localhost/U", "admin", "admin");
			V1Adapter v1Adapter = new V1Adapter(v1Instance);
			var scopeMoniker = args[0];

			Project project = new ProjectResolver(v1Instance).Resolve(scopeMoniker);
			Output(project);

			EpicGenerator generator = new EpicGenerator(project, v1Adapter);

			var count = 0;
			foreach (var theme in generator.ChooseThemes())
			{
				Output(theme);
				++count;
			}
			Console.WriteLine("{0} Themes", count);
		}

		private static void Output(BaseAsset asset)
		{
			Console.WriteLine("{0} {1}", asset.ID, asset.Name);
		}
	}
}
