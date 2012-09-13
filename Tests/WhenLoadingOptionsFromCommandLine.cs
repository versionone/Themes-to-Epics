using System.Collections.Specialized;
using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenLoadingOptionsFromCommandLine
	{
		private static Options _options;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenOptionsPreloadedFromSettings();
			WhenLoadingOptions();
		}

		private void GivenOptionsPreloadedFromSettings()
		{
			var settings = new NameValueCollection
			{
				{"Url", "abc"},
				{"Username", "def"},
				{"Password", "ghi"},
			};
			_options = new Options().Load(settings);
		}

		private void WhenLoadingOptions()
		{
			string[] args = new[] {"Scope:0", "xyz"};
			_options = _options.Load(args);
		}

		[Test]
		public void FirstArgumentIsScope()
		{
			Assert.That(_options.Scope, Is.EqualTo("Scope:0"));
		}

		[Test]
		public void SecondArgumentOverridesUrl()
		{
			Assert.That(_options.Url, Is.EqualTo("xyz"));
		}

		[Test]
		public void UsernameIsNotChanged()
		{
			Assert.That(_options.Username, Is.EqualTo("def"));
		}

		[Test]
		public void PasswordIsNotChanged()
		{
			Assert.That(_options.Password, Is.EqualTo("ghi"));
		}
	}
}