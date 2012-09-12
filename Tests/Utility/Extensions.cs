using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests.Utility
{
	static class Extensions
	{
		internal static void PickAValue(this IListValueProperty property)
		{
			property.CurrentValue = Random.Value(property);
		}
	}
}