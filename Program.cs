using System;
using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;
using VersionOne.SDK.ObjectModel.Filters;

namespace VersionOne.Themes_to_Epics
{
	class Program
	{
		static void Main(string[] args)
		{
			V1Instance v1 = new V1Instance("http://localhost/U", "admin", "admin");
			ThemeFilter themeFilter = new ThemeFilter
			{
				//State = { State.Active },
			};
			ICollection<Theme> themes = v1.Get.Themes(themeFilter);
			foreach (var theme in themes)
			{
				Console.WriteLine(theme.Name);
			}
			Console.WriteLine("{0} Themes", themes.Count);
		}

		public static  Epic GenerateEpicFrom(Theme theme, V1Instance v1)
		{
			return v1.Create.Epic(theme.Name, theme.Project);
		}
	}
}
