using System.Collections.Specialized;
using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenLoadingOptionsFromAppSettings
	{
		private NameValueCollection _settings;
		private static Options _options;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenSomeSettings();
			WhenLoadingOptions();
		}

		private void GivenSomeSettings()
		{
			_settings = new NameValueCollection
			{
				{"Url", "abc"},
				{"Username", "def"},
				{"Password", "ghi"},
				{"Bogus", "xyz"},
			};
		}

		private void WhenLoadingOptions()
		{
			_options = new Options().Load(_settings);
		}

		[Test]
		public void UrlIsRecognized()
		{
			Assert.That(_options.Url, Is.EqualTo("abc"));
		}

		[Test]
		public void UsernameIsRecognized()
		{
			Assert.That(_options.Username, Is.EqualTo("def"));
		}

		[Test]
		public void PasswordIsRecognized()
		{
			Assert.That(_options.Password, Is.EqualTo("ghi"));
		}
	}
}