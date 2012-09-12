using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenFindingWhichEpicWasGeneratedFromATheme : TesterBase
	{
		private Theme _theme;
		private Epic _epic;

		private void GivenATheme()
		{
			_theme = NewTheme();
		}

		private void GivenAnEpicHasBeenGenerated()
		{
			_epic = new EpicGenerator(this).GenerateEpicFrom(_theme);
		}

		private void WhenFindingTheGeneratedEpic()
		{
			_epic = new EpicGenerator(this).FindEpicGeneratedFrom(_theme);
		}

		[Test]
		public void NoEpicShouldBeFoundForANewTheme()
		{
			GivenATheme();
			WhenFindingTheGeneratedEpic();
			Assert.That(_epic, Is.Null);
		}

		[Test]
		public void AnEpicShouldBeFoundForAConvertedTheme()
		{
			GivenATheme();
			GivenAnEpicHasBeenGenerated();
			WhenFindingTheGeneratedEpic();
			Assert.That(_epic, Is.Not.Null);
		}
	}
}