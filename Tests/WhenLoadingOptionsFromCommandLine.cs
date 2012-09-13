using System.Collections.Specialized;
using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenLoadingOptionsFromCommandLine
	{
		private Options _options;

		[SetUp]
		public void SetUp()
		{
			GivenOptionsPreloadedFromSettings();
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

		private void Load(params string[] args)
		{
			_options.Load(args);
		}

		[Test]
		public void FirstArgumentIsScope()
		{
			Load("Scope:0");
			Assert.That(_options.Scope, Is.EqualTo("Scope:0"));
		}

		[Test]
		public void SecondArgumentIsUrl()
		{
			Load("Scope:0", "xyz");
			Assert.That(_options.Url, Is.EqualTo("xyz"));
		}

		[Test]
		public void ThirdArgumentIsUsername()
		{
			Load("Scope:0", "xyz", "uvw");
			Assert.That(_options.Username, Is.EqualTo("uvw"));
		}

		[Test]
		public void FourthArgumentIsPassword()
		{
			Load("Scope:0", "xyz", "uvw", "rst");
			Assert.That(_options.Password, Is.EqualTo("rst"));
		}

		[Test]
		public void MissingUrlDefaultsToAppSettings()
		{
			Load("Scope:0");
			Assert.That(_options.Url, Is.EqualTo("abc"));
		}

		[Test]
		public void MissingUsernameDefaultsToAppSettings()
		{
			Load("Scope:0", "xyz");
			Assert.That(_options.Username, Is.EqualTo("def"));
		}

		[Test]
		public void MissingPasswordDefaultsToAppSettings()
		{
			Load("Scope:0", "xyz");
			Assert.That(_options.Password, Is.EqualTo("ghi"));
		}
	}
}