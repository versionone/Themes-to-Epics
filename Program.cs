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
			return 0;
		}

		private static void Run(Options options)
		{
			Console.WriteLine("Connecting to {0} ...", options.Url);
			V1Instance v1Instance = new V1Instance(options.Url, options.Username, options.Password);
			V1Adapter v1Adapter = new V1Adapter(v1Instance);
			Console.WriteLine("Connected.");

			Console.WriteLine("Resolving project {0} ...", options.Scope);
			Project project = new ProjectResolver(v1Instance).Resolve(options.Scope);
			Console.WriteLine("Resolved to \"{0}\" ({1}).", project.Name, project.ID);

			EpicGenerator generator = new EpicGenerator(project, v1Adapter);

			Console.WriteLine("Processing themes...");

			IDictionary<Theme, Epic> map = new Dictionary<Theme, Epic>();
			foreach (var theme in generator.ChooseThemes())
			{
				var epic = generator.GenerateEpicFrom(theme);
				map.Add(theme, epic);
				Console.WriteLine("\t{0} -> {1}", Reference(theme), Reference(epic));
			}

			Console.WriteLine("{0} themes processed.", map.Count);

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
						epic.Parent = parentEpic;
						epic.Save();
						++count;
						Console.WriteLine("\t{0} -> {1}", Reference(epic), Reference(parentEpic));
					}
				}
			}

			Console.WriteLine("{0} generated epics parented.", count);

			Console.WriteLine("Assigning root, themed epics to generated epics");
			count = 0;

			foreach (Epic epic in generator.ChooseEpics())
			{
				var theme = epic.Theme;
				Epic newParentEpic;
				if (map.TryGetValue(theme, out newParentEpic))
				{
					epic.Parent = newParentEpic;
					epic.Save();
					++count;
					Console.WriteLine("\t{0} -> {1}", Reference(epic), Reference(newParentEpic));
				}
			}

			Console.WriteLine("{0} root, themed epics assigned.", count);
		}

		private static string Reference(ProjectAsset asset)
		{
			return string.Format("\"{0}\" ({1}/{2})", asset.Name, asset.DisplayID, asset.ID);
		}
	}
}
