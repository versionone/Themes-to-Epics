using System;
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
			Project project;
			try
			{
				project = _v1.Get.ProjectByID(moniker);
			}
			catch (Exception e)
			{
				throw new NotFoundException(moniker, e);
			}

			if (project == null)
				throw new NotFoundException(moniker);
			return project;
		}


		public class NotFoundException : Exception
		{
			public NotFoundException(string moniker) : this(moniker, null)
			{
			}

			public NotFoundException(string moniker, Exception innerException) 
				: base(MakeMessage(moniker), innerException)
			{
			}

			private static string MakeMessage(string moniker)
			{
				return string.Format("No project found matching '{0}'", moniker);
			}
		}
	}
}