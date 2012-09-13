using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	class ProjectResolver
	{
		private readonly V1Instance _v1;

		public ProjectResolver(V1Instance v1)
		{
			_v1 = v1;
		}

		public Project Resolve(string moniker)
		{
			var assetID = AssetID.FromToken(moniker);
			return _v1.Get.ProjectByID(assetID);
		}
	}
}