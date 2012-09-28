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
			var filter = new EpicFilter
			{
				Reference = { ReferenceToTheme(theme) }
			};
			return theme.Project.GetEpics(filter).FirstOrDefault();
		}

		public Epic GenerateEpicFrom(Theme theme)
		{
			Epic epic = FindEpicGeneratedFrom(theme) ?? _v1.CreateEpic(theme.Name, theme.Project);
			epic.Description = theme.Description;
			epic.Owners.Clear();
			foreach (var owner in theme.Owners)
				epic.Owners.Add(owner);
			epic.Risk.CurrentValue = theme.Risk.CurrentValue;
			epic.Priority.CurrentValue = theme.Priority.CurrentValue;
			epic.Estimate = theme.Estimate;
			epic.Goals.Clear();
			foreach (var goal in theme.Goals)
				epic.Goals.Add(goal);
			foreach (var customField in Configuration.Default.CustomFields)
			{
				epic.CustomDropdown[customField.ToEpic].CurrentValue = theme.CustomDropdown[customField.FromTheme].CurrentValue;
			}
			epic.Reference = ReferenceToTheme(theme);
			epic.Save();
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
			EpicFilter filter = new EpicFilter
			{
				Parent = {null},
				State = {State.Active},
			};
			return _scope.GetEpics(filter, true).Where(epic => epic.Theme != null);
		}
	}
}