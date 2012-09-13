using System.Collections.Specialized;
using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenLoadingInvalidOptionsFromAppSettings
	{
		private NameValueCollection _settings;

		private void GivenInvalidSettings()
		{
			_settings = new NameValueCollection
			{
				{"Bogus", "xyz"},
			};
		}

		private void WhenLoadingOptions()
		{
			new Options().Load(_settings);
		}

		[Test]
		public void UrlIsRecognized()
		{
			GivenInvalidSettings();
			Assert.That(() => WhenLoadingOptions(), Throws.Nothing);
		}
	}
}