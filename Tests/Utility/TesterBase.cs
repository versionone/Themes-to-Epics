using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
				_baseAssets[i].TryDelete();
			}
		}

		private T Push<T>(T baseAsset) where T : BaseAsset
		{
			_baseAssets.Add(baseAsset);
			return baseAsset;
		}

		protected V1Instance V1
		{
			get { return _v1 ?? (_v1 = CreateV1Instance()); }
		}

		private static V1Instance CreateV1Instance()
		{
			var options = new Options().Load(ConfigurationManager.AppSettings);
			return new V1Instance(options.Url, options.Username, options.Password);
		}

		protected Project TheProject
		{
			get { return _theProject ?? (_theProject = NewProject());}
		}

		protected Project NewProject()
		{
			return Push(V1.Create.Project(Random.Name(), "Scope:0", DateTime.Now, null));
		}

		protected Project NewProjectUnder(Project parent)
		{
			return Push(V1.Create.Project(Random.Name(), parent, DateTime.Now, null));
		}

		protected Member NewMember()
		{
			return Push(V1.Create.Member(Random.Name(), Random.Name()));
		}

		protected Theme NewTheme()
		{
			return NewThemeIn(TheProject);
		}

		protected Theme NewThemeIn(Project project)
		{
			return Push(V1.Create.Theme(Random.Name(), project));
		}

		protected Goal NewGoal()
		{
			return Push(V1.Create.Goal(Random.Name(), TheProject));
		}

		protected Epic NewEpic()
		{
			return NewEpicIn(TheProject);
		}

		protected Epic NewEpicIn(Project project)
		{
			return Push(V1.Create.Epic(Random.Name(), project));
		}


		internal EpicGenerator ClassUnderTest
		{
			get { return new EpicGenerator(TheProject, CustomFields, this); }
		}

		private static IEnumerable<ICopyCustomField> CustomFields
		{
			get { return Configuration.Default.CustomFields; }
		}

		protected static IEnumerable<ICopyCustomField> CustomDropDownFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.DropDown); }
		}

		protected static IEnumerable<ICopyCustomField> CustomTextFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.Text); }
		}

		protected static IEnumerable<ICopyCustomField> CustomCheckboxFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.Checkbox); }
		}

		protected static IEnumerable<ICopyCustomField> CustomNumberFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.Number); }
		}

		protected static IEnumerable<ICopyCustomField> CustomDateFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.Date); }
		}

		protected static IEnumerable<ICopyCustomField> CustomRichTextFields
		{
			get { return CustomFields.Where(c => c.Type == CustomFieldType.RichText); }
		}

		Epic IV1Adapter.CreateEpic(string name, Project project)
		{
			return Push(V1.Create.Epic(name, project));
		}
	}
}