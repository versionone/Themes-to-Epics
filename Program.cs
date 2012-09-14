using System;
using System.Collections.Generic;
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
			Console.WriteLine("Done.");
			return 0;
		}

		private static void Run(Options options)
		{
			var v1Instance = Connect(options);

			var project = Resolve(options.Scope, v1Instance);

			var generator = new EpicGenerator(project, new V1Adapter(v1Instance));

			IDictionary<Theme, Epic> map = new Dictionary<Theme, Epic>();

			Convert(generator, map);

			Tree(map);

			Reassign(generator, map);
		}

		private static V1Instance Connect(Options options)
		{
			Console.WriteLine("Connecting to {0} ...", options.Url);
			V1Instance v1Instance = new V1Instance(options.Url, options.Username, options.Password);
			Console.WriteLine("Connected.");
			return v1Instance;
		}

		private static Project Resolve(string moniker, V1Instance v1Instance)
		{
			Console.WriteLine("Resolving {0} ...", moniker);
			Project project = new ProjectResolver(v1Instance).Resolve(moniker);
			Console.WriteLine("Resolved to {0}.", Reference(project));
			return project;
		}

		private static void Convert(EpicGenerator generator, IDictionary<Theme, Epic> map)
		{
			Console.WriteLine("Processing themes...");

			foreach (var theme in generator.ChooseThemes())
			{
				Console.Write("\t{0} -> ", Reference(theme));
				var epic = generator.GenerateEpicFrom(theme);
				map.Add(theme, epic);
				Console.WriteLine(Reference(epic));
			}

			Console.WriteLine("{0} themes processed.", map.Count);
		}

		private static void Tree(IDictionary<Theme, Epic> map)
		{
			Console.WriteLine("Parenting generated epics...");
			var count = 0;

			foreach (KeyValuePair<Theme, Epic> pair in map)
			{
				var theme = pair.Key;
				var epic = pair.Value;
				var parentTheme = theme.ParentTheme;
				if (parentTheme != null)
				{
					Epic parentEpic;
					if (map.TryGetValue(parentTheme, out parentEpic))
					{
						Console.WriteLine("\t{0} -> {1}", Reference(epic), Reference(parentEpic));
						epic.Parent = parentEpic;
						epic.Save();
						++count;
					}
				}
			}

			Console.WriteLine("{0} generated epics parented.", count);
		}

		private static void Reassign(EpicGenerator generator, IDictionary<Theme, Epic> map)
		{
			Console.WriteLine("Assigning existing epics to generated epics");
			var count = 0;

			foreach (Epic epic in generator.ChooseEpics())
			{
				var theme = epic.Theme;
				Epic newParentEpic;
				if (map.TryGetValue(theme, out newParentEpic))
				{
					Console.WriteLine("\t{0} -> {1}", Reference(epic), Reference(newParentEpic));
					epic.Parent = newParentEpic;
					epic.Save();
					++count;
				}
			}

			Console.WriteLine("{0} existing epics assigned to generated epics.", count);
		}

		private static string Reference(BaseAsset asset)
		{
			return string.Format("\"{0}\" ({1})", asset.Name, asset.ID);
		}

		private static string Reference(ProjectAsset asset)
		{
			return string.Format("\"{0}\" ({1}/{2})", asset.Name, asset.DisplayID, asset.ID);
		}
	}
}
