using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicFromAThemeAgain : TesterBase
	{
		private Theme _theme;
		private Epic _epic1;
		private Epic _epic2;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenATheme();
			GivenAnEpicHasBeenGenerated();
			GivenTheThemeHasBeenEdited();
			WhenGeneratingAnEpicAgain();
		}

		private void GivenATheme()
		{
			_theme = NewTheme();
			_theme.Owners.Add(NewMember());
			_theme.Owners.Add(NewMember());
			_theme.Goals.Add(NewGoal());
			_theme.Goals.Add(NewGoal());
			_theme.Save();
		}

		private void GivenAnEpicHasBeenGenerated()
		{
			_epic1 = ClassUnderTest.GenerateEpicFrom(_theme);
		}

		private void GivenTheThemeHasBeenEdited()
		{
			_theme.Owners.Clear();
			_theme.Owners.Add(NewMember());
			_theme.Goals.Clear();
			_theme.Goals.Add(NewGoal());
			_theme.Save();
		}

		private void WhenGeneratingAnEpicAgain()
		{
			_epic2 = ClassUnderTest.GenerateEpicFrom(_theme);
		}

		[Test]
		public void TheOldEpicShouldBeReturnedInstead()
		{
			Assert.That(_epic2, Is.EqualTo(_epic1));
		}

		[Test]
		public void TheEpicShouldHaveTheEditedOwners()
		{
			Assert.That(_epic2.Owners, Is.EquivalentTo(_theme.Owners));
		}

		[Test]
		public void TheEpicShouldHaveTheEditedGoals()
		{
			Assert.That(_epic2.Goals, Is.EquivalentTo(_theme.Goals));
		}
	}
}