using System;
using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests.Utility
{
	public abstract class TesterBase : IDisposable, IV1Adapter
	{
		private V1Instance _v1;
		private IList<BaseAsset> _baseAssets = new List<BaseAsset>();
		private Project _theProject;

		void IDisposable.Dispose()
		{
			int i = _baseAssets.Count;
			while (--i >= 0)
			{
				BaseAsset baseAsset = _baseAssets[i];
				if (baseAsset != null)
					baseAsset.Delete();
			}
		}

		private T Push<T>(T baseAsset) where T : BaseAsset
		{
			_baseAssets.Add(baseAsset);
			return baseAsset;
		}

		private V1Instance V1
		{
			get { return _v1 ?? (_v1 = new V1Instance("http://localhost/U", "admin", "admin")); }
		}

		protected Project TheProject
		{
			get { return _theProject ?? (_theProject = NewProject());}
		}

		protected Project NewProject()
		{
			return Push(V1.Create.Project(Random.Name(), "Scope:0", DateTime.Now, null));
		}

		protected Project NewProject(Project parent)
		{
			return Push(V1.Create.Project(Random.Name(), parent, DateTime.Now, null));
		}

		protected Member NewMember()
		{
			return Push(V1.Create.Member(Random.Name(), Random.Name()));
		}

		protected Theme NewTheme()
		{
			return NewTheme(TheProject);
		}

		protected Theme NewTheme(Project project)
		{
			return Push(V1.Create.Theme(Random.Name(), project));
		}

		protected Goal NewGoal()
		{
			return Push(V1.Create.Goal(Random.Name(), TheProject));
		}

		protected Epic NewEpic()
		{
			return NewEpic(TheProject);
		}

		protected Epic NewEpic(Project project)
		{
			return Push(V1.Create.Epic(Random.Name(), project));
		}

		internal EpicGenerator ClassUnderTest
		{
			get { return new EpicGenerator(TheProject, this); }
		}

		Epic IV1Adapter.CreateEpic(string name, Project project)
		{
			return Push(V1.Create.Epic(name, project));
		}
	}
}