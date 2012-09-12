using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenChoosingThemesToConvert : TesterBase
	{
		private Theme _theme;
		private IEnumerable<Theme> _themes;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenATheme();
			WhenChoosingThemes();
		}

		private void GivenATheme()
		{
			_theme = NewTheme();
		}

		private void WhenChoosingThemes()
		{
			_themes = new EpicGenerator(this).ChooseThemes(TheProject);
		}

		[Test]
		public void TheThemeShouldBeIncluded()
		{
			Assert.That(_themes, Has.Member(_theme));
		}
	}
}