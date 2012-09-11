using System;
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
		}

		private IListValueProperty PickRisk()
		{
			return null;
		}

		private void WhenGeneratingAnEpic()
		{
			_epic = Queue(new EpicGenerator(V1).From(_theme));
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
	}
}