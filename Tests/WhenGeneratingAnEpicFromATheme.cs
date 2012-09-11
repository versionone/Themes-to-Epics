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
		public void Setup()
		{
			GivenATheme();
			WhenGeneratingAnEpic();
		}

		private void GivenATheme()
		{
			_theme = V1.Create.Theme(Random.Name(), NewProject());
		}

		private void WhenGeneratingAnEpic()
		{
			_epic = Program.GenerateEpicFrom(_theme, V1);
		}

		[Test]
		public void TheEpicShouldHaveTheSameName()
		{
			Assert.That(_epic.Name, Is.EqualTo(_theme.Name));
		}
	}
}