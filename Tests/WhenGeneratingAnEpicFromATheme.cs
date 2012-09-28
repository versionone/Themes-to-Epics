using System.Linq;
using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicFromATheme : TesterBase
	{
		private Theme _theme;
		private Epic _epic;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenATheme();
			WhenGeneratingAnEpic();
		}

		private void GivenATheme()
		{
			_theme = NewTheme();
			_theme.Description = Random.Description();
			_theme.Owners.Add(NewMember());
			_theme.Owners.Add(NewMember());
			_theme.Risk.PickAValue();
			_theme.Priority.PickAValue();
			_theme.Estimate = Random.Estimate();
			_theme.Goals.Add(NewGoal());
			_theme.Goals.Add(NewGoal());
			foreach (var customField in CustomDropDownFields)
				_theme.CustomDropdown[customField.FromTheme].PickAValue();
			_theme.Save();
		}

		private void WhenGeneratingAnEpic()
		{
			_epic = ClassUnderTest.GenerateEpicFrom(_theme);
		}

		[Test]
		public void TheEpicShouldHaveTheSameName()
		{
			Assert.That(_epic.Name, Is.EqualTo(_theme.Name));
		}

		[Test]
		public void TheEpicShouldHaveTheSameProject()
		{
			Assert.That(_epic.Project, Is.EqualTo(_theme.Project));
		}

		[Test]
		public void TheEpicShouldHaveTheSameDescription()
		{
			Assert.That(_epic.Description, Is.EqualTo(_theme.Description));
		}

		[Test]
		public void TheEpicShouldHaveTheSameOwners()
		{
			Assert.That(_epic.Owners, Is.EquivalentTo(_theme.Owners));
		}

		[Test]
		public void TheEpicShouldHaveTheSameRisk()
		{
			Assert.That(_epic.Risk.CurrentValue, Is.EqualTo(_theme.Risk.CurrentValue));
		}

		[Test]
		public void TheEpicShouldHaveTheSamePriority()
		{
			Assert.That(_epic.Priority.CurrentValue, Is.EqualTo(_theme.Priority.CurrentValue));
		}

		[Test]
		public void TheEpicShouldHaveTheSameEstimate()
		{
			Assert.That(_epic.Estimate, Is.EqualTo(_theme.Estimate));
		}

		[Test]
		public void TheEpicShouldHaveTheSameGoals()
		{
			Assert.That(_epic.Goals, Is.EquivalentTo(_theme.Goals));
		}

		[Test]
		public void TheEpicReferenceFieldShouldContainTheThemeID()
		{
			Assert.That(_epic.Reference, Contains.Substring(_theme.DisplayID));
		}

		[Test]
		public void TheEpicShouldHaveNoTheme()
		{
			Assert.That(_epic.Theme, Is.Null);
		}

		[Test]
		public void TheEpicShouldHaveTheSameCustomDropdownValues()
		{
			if (!CustomDropDownFields.Any())
				Assert.Ignore("No custom dropdown fields defined");
			foreach (var customField in CustomDropDownFields)
			{
				var epicCustomValue = _epic.CustomDropdown[customField.ToEpic].CurrentValue;
				var themeCustomValue = _theme.CustomDropdown[customField.FromTheme].CurrentValue;
				Assert.That(epicCustomValue, Is.EqualTo(themeCustomValue));
			}
		}
	}
}