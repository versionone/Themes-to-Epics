using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenChoosingThemesToConvert : TesterBase
	{
		private Theme _theme1;
		private Theme _theme2;
		private IEnumerable<Theme> _themes;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenThemesInAProject();
			WhenChoosingThemesInTheProject();
		}

		private void GivenThemesInAProject()
		{
			_theme1 = NewTheme();
			_theme2 = NewTheme();
		}

		private void WhenChoosingThemesInTheProject()
		{
			_themes = new EpicGenerator(this).ChooseThemes(TheProject);
		}

		[Test]
		public void AllThemesInTheProjectShouldBeChosen()
		{
			Assert.That(_themes, Has.Member(_theme1).And.Member(_theme2));
		}
	}
}