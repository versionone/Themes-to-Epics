using System.Collections.Generic;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics
{
	class EpicGenerator
	{
		private readonly V1Instance _v1;

		public EpicGenerator(V1Instance v1)
		{
			_v1 = v1;
		}

		public Epic From(Theme theme)
		{
			var attributes = new Dictionary<string, object>()
			{
				{"Description", theme.Description},
			};
			return _v1.Create.Epic(theme.Name, theme.Project, attributes);
		}
	}
}