using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenGeneratingAnEpicTreeFromAThemeTree : TesterBase
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
			_epic = ClassUnderTest.GenerateEpicTreeFrom(_theme);
		}

		[Test]
		public void TheEpicShouldHaveEpicChildren()
		{
			Assert.That(_epic.GetChildEpics(null).Count, Is.EqualTo(_theme.GetChildThemes(null).Count));
		}
	}
}