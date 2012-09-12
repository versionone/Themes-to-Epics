using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	class EpicGenerator
	{
		private readonly IV1Adapter _v1;

		public EpicGenerator(IV1Adapter v1)
		{
			_v1 = v1;
		}

		public Epic From(Theme theme)
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
			return epic;
		}
	}
}