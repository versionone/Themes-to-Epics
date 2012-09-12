using NUnit.Framework;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicFromATheme : WithV1Instance
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
		}

		private void WhenGeneratingAnEpic()
		{
			_epic = new EpicGenerator(this).From(_theme);
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
	}

	[TestFixture]
	public class WhenGeneratingAnEpicTreeFromAThemeTree : WithV1Instance
	{
		private Theme _theme;
		private Epic _epic;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenAThemeTree();
			WhenGeneratingAnEpicTree();
		}

		private void GivenAThemeTree()
		{
			_theme = NewTheme();

			var child1 = NewTheme();
			child1.ParentTheme = _theme;
			child1.Save();

			var child2 = NewTheme();
			child2.ParentTheme = _theme;
			child2.Save();

			var grandchild = NewTheme();
			grandchild.ParentTheme = child2;
			grandchild.Save();
		}

		private void WhenGeneratingAnEpicTree()
		{
			_epic = new EpicGenerator(this).From(_theme);
		}


		[Test]
		public void TheEpicShouldHaveEpicChildren()
		{
			Assert.That(_epic.GetChildEpics(null).Count, Is.EqualTo(_theme.GetChildThemes(null).Count));
		}
	}
}