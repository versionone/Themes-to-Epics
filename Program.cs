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
			V1Instance v1Instance = new V1Instance("http://localhost/U", "admin", "admin");
			V1Adapter v1Adapter = new V1Adapter(v1Instance);
			var scopeMoniker = args[0];
			Project project = new ProjectResolver(v1Instance).Resolve(scopeMoniker);
			EpicGenerator generator = new EpicGenerator(project, v1Adapter);

			ThemeFilter themeFilter = new ThemeFilter
			{
				//State = { State.Active },
			};
			ICollection<Theme> themes = v1Instance.Get.Themes(themeFilter);
			foreach (var theme in themes)
			{
				Console.WriteLine(theme.Name);
			}
			Console.WriteLine("{0} Themes", themes.Count);
		}
	}
}
