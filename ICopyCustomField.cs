namespace VersionOne.Themes_to_Epics
{
	public enum CustomFieldType
	{
		DropDown,
	}

	public interface ICopyCustomField
	{
		CustomFieldType Type { get; }
		string FromTheme { get; }
		string ToEpic { get; }
	}
}