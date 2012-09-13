using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.ObjectModel;
using VersionOne.SDK.ObjectModel.Filters;

namespace VersionOne.Themes_to_Epics
{
	class EpicGenerator
	{
		private readonly IV1Adapter _v1;
		private readonly Project _scope;

		public EpicGenerator(Project scope, IV1Adapter v1)
		{
			_v1 = v1;
			_scope = scope;
		}

		private static string ReferenceToTheme(Theme theme)
		{
			return "generated-from:" + theme.DisplayID + "/" + theme.ID;
		}

		public Epic FindEpicGeneratedFrom(Theme theme)
		{
			var filter = new EpicFilter()
			{
				Reference = { ReferenceToTheme(theme) }
			};
			return theme.Project.GetEpics(filter).FirstOrDefault();
		}

		public Epic GenerateEpicFrom(Theme theme)
		{
			Epic epic = _v1.CreateEpic(theme.Name, theme.Project);
			epic.Description = theme.Description;
			foreach (var owner in theme.Owners)
				epic.Owners.Add(owner);
			epic.Risk.CurrentValue = theme.Risk.CurrentValue;
			epic.Priority.CurrentValue = theme.Priority.CurrentValue;
			epic.Estimate = theme.Estimate;
			foreach (var goal in theme.Goals)
				epic.Goals.Add(goal);
			epic.Reference = ReferenceToTheme(theme);
			epic.Save();
			return epic;
		}

		public Epic GenerateEpicTreeFrom(Theme theme)
		{
			Epic epic = GenerateEpicFrom(theme);
			foreach (var childTheme in theme.GetChildThemes(null))
			{
				var childEpic = GenerateEpicTreeFrom(childTheme);
				childEpic.Parent = epic;
				childEpic.Save();
			}
			return epic;
		}

		public IEnumerable<Theme> ChooseThemes()
		{
			ThemeFilter filter = new ThemeFilter
			{
				State = {State.Active}
			};
			return _scope.GetThemes(filter, true);
		}

		public IEnumerable<Epic> ChooseEpics()
		{
			return _scope.GetEpics(null);
		}
	}
}