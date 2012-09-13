using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests.Utility
{
	static class Extensions
	{
		internal static void PickAValue(this IListValueProperty property)
		{
			property.CurrentValue = Random.Value(property);
		}

		internal static Epic ChildOf(this Epic epic, Epic parent)
		{
			epic.Parent = parent;
			epic.Save();
			return epic;
		}

		internal static Epic WithTheme(this Epic epic, Theme theme)
		{
			epic.Theme = theme;
			epic.Save();
			return epic;
		}
	}
}