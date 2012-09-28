namespace VersionOne.Themes_to_Epics
{
	public enum CustomFieldType
	{
		DropDown,
		Text,
		Checkbox,
		Number,
		Date,
		RichText,
	}

	public interface ICopyCustomField
	{
		CustomFieldType Type { get; }
		string FromTheme { get; }
		string ToEpic { get; }
	}
}