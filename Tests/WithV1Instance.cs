using System;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests
{
	public abstract class WithV1Instance
	{
		private V1Instance _v1;

		protected V1Instance V1
		{
			get { return _v1 ?? (_v1 = new V1Instance("http://localhost/U", "admin", "admin")); }
		}

		protected Project NewProject()
		{
			return V1.Create.Project(Random.Name(), "Scope:0", DateTime.Now, null);
		}
	}
}