using NUnit.Framework;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicFromATheme : WithV1Instance
	{
		private Project _project;
		private Theme _theme;
		private Epic _epic;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_project = NewProject();
			_theme = V1.Create.Theme(Random.Name(), _project);
			_epic = Program.GenerateEpicFrom(_theme, V1);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (_epic != null) _epic.Delete();
			if (_theme != null) _theme.Delete();
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
	}
}