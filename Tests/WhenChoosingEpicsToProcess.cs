using System.Collections.Generic;
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
		private IEnumerable<Epic> _epics;

		[TestFixtureSetUp]
		public void SetUp()
		{
			GivenEpicsInTheProject();
			WhenChoosingEpics();
		}

		private void GivenEpicsInTheProject()
		{
			_epic1 = NewEpic();
			_epic2 = NewEpic();
		}

		private void WhenChoosingEpics()
		{
			_epics = new EpicGenerator(this).ChooseEpics(TheProject);
		}

		[Test]
		public void EpicsInTheProjectShouldBeChosen()
		{
			Assert.That(_epics, Has.Member(_epic1).And.Member(_epic2));
		}
	}
}