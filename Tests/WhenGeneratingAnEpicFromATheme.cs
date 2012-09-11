using NUnit.Framework;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicFromATheme : WithV1Instance
	{
		private Project _project;
		private Member _owner1;
		private Member _owner2;
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
			_project = NewProject();
			_owner1 = NewMember();
			_owner2 = NewMember();
			_theme = V1.Create.Theme(Random.Name(), _project);
			_theme.Description = Random.String(20);
			_theme.Owners.Add(_owner1);
			_theme.Owners.Add(_owner2);
		}

		private void WhenGeneratingAnEpic()
		{
			_epic = new EpicGenerator(V1).From(_theme);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (_epic != null) _epic.Delete();
			if (_theme != null) _theme.Delete();
			if (_owner2 != null) _owner2.Delete();
			if (_owner1 != null) _owner1.Delete();
			if (_project != null) _project.Delete();
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
	}
}