using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	interface IV1Adapter
	{
		Epic CreateEpic(string name, Project project);
	}

	internal class V1Adapter : IV1Adapter
	{
		private readonly V1Instance _v1;

		public V1Adapter(V1Instance v1)
		{
			_v1 = v1;
		}

		public Epic CreateEpic(string name, Project project)
		{
			return _v1.Create.Epic(name, project);
		}
	}
}