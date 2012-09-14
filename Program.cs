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
				Run(options);
			}
			catch (Options.InvalidOptionsException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(Options.Usage());
				return 1;
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.Message);
				return 2;
			}
			return 0;
		}

		private static void Run(Options options)
		{
			V1Instance v1Instance = new V1Instance(options.Url, options.Username, options.Password);
			V1Adapter v1Adapter = new V1Adapter(v1Instance);

			Project project = new ProjectResolver(v1Instance).Resolve(options.Scope);
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
