using System;
using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	internal class Conversion
	{
		private readonly Options _options;
		private readonly IDictionary<Theme, Epic> _map = new Dictionary<Theme, Epic>();
		private EpicGenerator _generator;

		public Conversion(Options options)
		{
			_options = options;
		}

		public void Run()
		{
			var v1Instance = Connect();
			var project = Resolve(_options.Scope, v1Instance);
			_generator = new EpicGenerator(project, new V1Adapter(v1Instance));
			Convert();
			Tree();
			Reassign();
		}

		private V1Instance Connect()
		{
			Console.WriteLine("Connecting to {0} ...", _options.Url);
			V1Instance v1Instance = new V1Instance(_options.Url, _options.Username, _options.Password);
			Console.WriteLine("Connected.");
			return v1Instance;
		}

		private Project Resolve(string moniker, V1Instance v1Instance)
		{
			Console.WriteLine("Resolving {0} ...", moniker);
			Project project = new ProjectResolver(v1Instance).Resolve(moniker);
			Console.WriteLine("Resolved to {0}.", Reference(project));
			return project;
		}

		private void Convert()
		{
			Console.WriteLine("Processing themes...");

			foreach (var theme in _generator.ChooseThemes())
			{
				Console.Write("\t{0} -> ", Reference(theme));
				Epic epic = _generator.GenerateEpicFrom(theme);
				_map.Add(theme, epic);
				Console.WriteLine(Reference(epic));
			}

			Console.WriteLine("{0} themes processed.", _map.Count);
		}

		private void Tree()
		{
			Console.WriteLine("Parenting generated epics...");
			var count = 0;

			foreach (KeyValuePair<Theme, Epic> pair in _map)
			{
				Theme theme = pair.Key;
				Epic epic = pair.Value;
				Theme parentTheme = theme.ParentTheme;
				if (parentTheme != null)
				{
					Epic parentEpic;
					if (_map.TryGetValue(parentTheme, out parentEpic))
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

		private void Reassign()
		{
			Console.WriteLine("Assigning existing epics to generated epics");
			var count = 0;

			foreach (Epic epic in _generator.ChooseEpics())
			{
				Theme theme = epic.Theme;
				Epic newParentEpic;
				if (_map.TryGetValue(theme, out newParentEpic))
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