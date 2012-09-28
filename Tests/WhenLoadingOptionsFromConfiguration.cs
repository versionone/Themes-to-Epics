using System.Linq;
using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenLoadingOptionsFromConfiguration
	{
		private Configuration _config;
		private Options _options;

		private void GivenNullConfiguration()
		{
			_config = null;
		}

		private void GivenEmptyConfiguration()
		{
			_config = new Configuration();
		}

		private void GivenValidConfiguration()
		{
			_config = new Configuration
			{
				CustomFields = 
				{
					new Configuration.CustomField{ Type = CustomFieldType.DropDown, FromTheme = "a", ToEpic = "b"},
					new Configuration.CustomField{ Type = CustomFieldType.DropDown, FromTheme = "c", ToEpic = "d"},
				}
			};
		}

		private void WhenLoadingOptions()
		{
			_options = new Options().Load(_config);
		}

		[Test]
		public void NullConfigurationResultsInEmptyCustomFields()
		{
			GivenNullConfiguration();
			WhenLoadingOptions();
			Assert.That(_options.CustomFields.Any(), Is.False);
		}

		[Test]
		public void EmptyConfigurationResultsInEmptyCustomFields()
		{
			GivenEmptyConfiguration();
			WhenLoadingOptions();
			Assert.That(_options.CustomFields.Any(), Is.False);
		}

		[Test]
		public void ValidCustomFieldsResultsInCustomFields()
		{
			GivenValidConfiguration();
			WhenLoadingOptions();
			Assert.That(_options.CustomFields.Select(f => f.FromTheme), Is.EquivalentTo(new[] {"a", "c"}));
		}
	}
}