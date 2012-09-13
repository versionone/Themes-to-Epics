using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenChoosingEpicsToProcess : TesterBase
	{
		private Epic _epic1;
		private Epic _epic2;
		private Epic _epicInChildProject;
		private Epic _epicInOtherProject;
		private IEnumerable<Epic> _epics;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenThemedEpicsInTheProject();
			GivenUnthemedEpicsInTheProject();
			GivenChildEpics();
			GivenEpicsInChildProjects();
			GivenEpicsInOtherProjects();
			GivenClosedEpics();
			WhenChoosingEpics();
		}

		private Epic NewThemedEpicIn(Project project)
		{
			return NewEpic(project)
				.WithTheme(NewTheme(project));
		}

		private void GivenThemedEpicsInTheProject()
		{
			_epic1 = NewThemedEpicIn(TheProject);
			_epic2 = NewThemedEpicIn(TheProject);
		}

		private void GivenUnthemedEpicsInTheProject()
		{
			NewEpic();
		}

		private void GivenChildEpics()
		{
			NewThemedEpicIn(TheProject).ChildOf(_epic1);
		}

		private void GivenEpicsInChildProjects()
		{
			_epicInChildProject = NewEpic(NewProjectUnder(TheProject)).WithTheme(NewTheme());
		}

		private void GivenEpicsInOtherProjects()
		{
			_epicInOtherProject = NewThemedEpicIn(NewProject());
		}

		private void GivenClosedEpics()
		{
			NewThemedEpicIn(TheProject).Close();
		}

		private void WhenChoosingEpics()
		{
			_epics = ClassUnderTest.ChooseEpics();
		}

		[Test]
		public void EpicsInTheProjectShouldBeChosen()
		{
			Assert.That(_epics, Has.Member(_epic1).And.Member(_epic2));
		}

		[Test]
		public void EpicsInChildProjectsShouldBeChosen()
		{
			Assert.That(_epics, Has.Member(_epicInChildProject));
		}

		[Test]
		public void EpicsInOtherProjectsShouldNotBeChosen()
		{
			Assert.That(_epics, Has.No.Member(_epicInOtherProject));
		}

		[Test]
		public void OnlyEpicsWithThemesShouldBeChosen()
		{
			Assert.That(_epics.Select(epic => epic.Theme), Has.All.Not.Null);
		}

		[Test]
		public void OnlyRootEpicsShouldBeChosen()
		{
			Assert.That(_epics.Select(epic => epic.Parent), Has.All.Null);
		}

		[Test]
		public void OnlyActiveEpicsShouldBeChosen()
		{
			Assert.That(_epics.Select(epic => epic.IsActive), Has.All.True);
		}
	}
}