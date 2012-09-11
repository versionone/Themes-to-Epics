using System;
using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	public abstract class WithV1Instance : IDisposable
	{
		private V1Instance _v1;
		private IList<BaseAsset> _baseAssets = new List<BaseAsset>();

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

		protected T Queue<T>(T baseAsset) where T : BaseAsset
		{
			_baseAssets.Add(baseAsset);
			return baseAsset;
		}

		protected V1Instance V1
		{
			get { return _v1 ?? (_v1 = new V1Instance("http://localhost/U", "admin", "admin")); }
		}

		protected Project NewProject()
		{
			return Queue(V1.Create.Project(Random.Name(), "Scope:0", DateTime.Now, null));
		}

		protected Member NewMember()
		{
			return Queue(V1.Create.Member(Random.Name(), Random.Name()));
		}

		protected Theme NewTheme()
		{
			return Queue(V1.Create.Theme(Random.Name(), NewProject()));
		}


	}
}