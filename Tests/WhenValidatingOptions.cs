using NUnit.Framework;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenValidatingOptions
	{
		[Test]
		public void MissingScopeThrowsAnError()
		{
			var options = new Options();
			Assert.That(() => options.Validate(), Throws.InstanceOf<Options.InvalidOptionsException>().And.Message.ContainsSubstring("scope"));
		}

		[Test]
		public void MissingUrlThrowsAnError()
		{
			var options = new Options().Load(new[] { "scope" });
			Assert.That(() => options.Validate(), Throws.InstanceOf<Options.InvalidOptionsException>().And.Message.ContainsSubstring("url"));
		}

		[Test]
		public void MissingUsernameThrowsAnError()
		{
			var options = new Options().Load(new[] { "scope", "url" });
			Assert.That(() => options.Validate(), Throws.InstanceOf<Options.InvalidOptionsException>().And.Message.ContainsSubstring("username"));
		}

		[Test]
		public void MissingPasswordThrowsAnError()
		{
			var options = new Options().Load(new[] { "scope", "url", "username" });
			Assert.That(() => options.Validate(), Throws.InstanceOf<Options.InvalidOptionsException>().And.Message.ContainsSubstring("password"));
		}

		[Test]
		public void AllArgumentsIsValid()
		{
			var options = new Options().Load(new[] { "scope", "url", "username", "password" });
			Assert.That(() => options.Validate(), Throws.Nothing);
		}
	}
}