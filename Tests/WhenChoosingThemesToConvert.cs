using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenChoosingThemesToConvert : TesterBase
	{
		private Theme _theme1;
		private Theme _theme2;
		private Theme _childTheme;
		private Theme _closedTheme;
		private Theme _themeInChildProject;
		private Theme _themeInOtherProject;
		private IEnumerable<Theme> _themes;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenThemesInTheProject();
			GivenChildrenThemes();
			GivenClosedThemes();
			GivenThemesInChildProjects();
			GivenThemesInOtherProjects();
			WhenChoosingThemes();
		}

		private void GivenThemesInTheProject()
		{
			_theme1 = NewTheme();
			_theme2 = NewTheme();
		}

		private void GivenChildrenThemes()
		{
			_childTheme = NewTheme();
			_childTheme.ParentTheme = _theme1;
			_childTheme.Save();
		}

		private void GivenClosedThemes()
		{
			_closedTheme = NewTheme();
			_closedTheme.Close();
		}

		private void GivenThemesInChildProjects()
		{
			_themeInChildProject = NewTheme(NewProject(TheProject));
		}

		private void GivenThemesInOtherProjects()
		{
			_themeInOtherProject = NewTheme(NewProject());
		}

		private void WhenChoosingThemes()
		{
			_themes = ClassUnderTest.ChooseThemes();
		}

		[Test]
		public void ThemesInTheProjectShouldBeChosen()
		{
			Assert.That(_themes, Has.Member(_theme1).And.Member(_theme2));
		}

		[Test]
		public void ChildrenThemesShouldBeChosen()
		{
			Assert.That(_themes, Has.Member(_childTheme));
		}

		[Test]
		public void ClosedThemesShouldNotBeChosen()
		{
			Assert.That(_themes, Has.No.Member(_closedTheme));
		}

		[Test]
		public void ThemesInChildProjectsShouldBeChosen()
		{
			Assert.That(_themes, Has.Member(_themeInChildProject));
		}

		[Test]
		public void ThemesInOtherProjectsShouldNotBeChosen()
		{
			Assert.That(_themes, Has.No.Member(_themeInOtherProject));
		}
	}
}