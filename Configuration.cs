using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace VersionOne.Themes_to_Epics
{
	public class Configuration : ConfigurationSection
	{
		public static Configuration Default
		{
			get
			{
				return ConfigurationManager.GetSection("VersionOne/Themes_to_Epics") as Configuration;
			}
		}

		[ConfigurationProperty("customFields")]
		public CustomFieldsConfiguration CustomFields
		{
			get { return (CustomFieldsConfiguration) this["customFields"]; }
		}

		[ConfigurationCollection(typeof(CustomField), AddItemName = "copy", CollectionType = ConfigurationElementCollectionType.BasicMap)]
		public class CustomFieldsConfiguration : ConfigurationElementCollection, IEnumerable<CustomField>
		{
			public new CustomField this[string from]
			{
				get { return BaseGet(from) as CustomField; }
			}

			protected override ConfigurationElement CreateNewElement()
			{
				return new CustomField();
			}

			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((CustomField)element).FromTheme;
			}

			public new IEnumerator<CustomField> GetEnumerator()
			{
				foreach (CustomField customField in (IEnumerable)this)
				{
					yield return customField;
				}
			}
		}

		public enum CustomFieldType
		{
			DropDown,
		}

		public class CustomField : ConfigurationElement
		{
			[ConfigurationProperty("type", IsKey = true, IsRequired = true)]
			public CustomFieldType Type
			{
				get { return (CustomFieldType)this["type"]; }
				set { this["type"] = value; }
			}

			[ConfigurationProperty("from-theme", IsKey = true, IsRequired = true)]
			public string FromTheme
			{
				get { return (string)this["from-theme"]; }
				set { this["from-theme"] = value; }
			}

			[ConfigurationProperty("to-epic", IsKey = false, IsRequired = true)]
			public string ToEpic
			{
				get { return (string)this["to-epic"]; }
				set { this["to-epic"] = value; }
			}
		}
	}
}