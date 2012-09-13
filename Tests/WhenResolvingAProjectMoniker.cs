using NUnit.Framework;
using VersionOne.SDK.ObjectModel;
using VersionOne.Themes_to_Epics.Tests.Utility;

namespace VersionOne.Themes_to_Epics.Tests
{
	[TestFixture]
	public class WhenResolvingAProjectMoniker : TesterBase
	{
		private Project _project;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_project = NewProject();
		}

		private Project Resolve(string moniker)
		{
			return new ProjectResolver(V1).Resolve(moniker);
		}

		[Test]
		public void AValidIDResolvesToTheProject()
		{
			Assert.That(Resolve(_project.ID), Is.EqualTo(_project));
		}

		[Test]
		public void ANonExistentIDThrowsAnException()
		{
			Assert.That(() => Resolve("Scope:9876543"), Throws.InstanceOf<ProjectResolver.NotFoundException>());
		}

		[Test]
		public void AnInvalidIDThrowsAnException()
		{
			Assert.That(() => Resolve("This:Is:Not:An:ID"), Throws.InstanceOf<ProjectResolver.NotFoundException>());
		}
	}
}