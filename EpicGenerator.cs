using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	class EpicGenerator
	{
		private readonly V1Instance _v1;

		public EpicGenerator(V1Instance v1)
		{
			_v1 = v1;
		}

		public Epic From(Theme theme)
		{
			var attributes = new Dictionary<string, object>
			{
				{"Description", theme.Description},
			};
			Epic epic = _v1.Create.Epic(theme.Name, theme.Project, attributes);
			foreach (var owner in theme.Owners)
				epic.Owners.Add(owner);
			epic.Risk.CurrentValue = theme.Risk.CurrentValue;
			epic.Priority.CurrentValue = theme.Priority.CurrentValue;
			epic.Estimate = theme.Estimate;
			foreach (var goal in theme.Goals)
				epic.Goals.Add(goal);
			return epic;
		}
	}
}