using System;
using System.Configuration;

namespace VersionOne.Themes_to_Epics
{
	public class Program
	{
		static int Main(string[] args)
		{
			try
			{
				Options options = new Options()
					.Load(ConfigurationManager.AppSettings)
					.Load(args)
					.Validate();
				new Conversion(options).Run();
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
			Console.WriteLine("Done.");
			return 0;
		}

	}
}
