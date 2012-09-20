using System.Text;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.Themes_to_Epics.Tests.Utility
{
	public static class Random
	{
		private static readonly System.Random _rnd = new System.Random();
		private const string _characters = "abcdefghijklmnopqrstuvwxyz_ABCDDEFHIJKLMNOPQRSTUVWXYZ_0123456789";

		public static char Character()
		{
			return _characters[_rnd.Next(_characters.Length)];
		}

		public static string Name()
		{
			return String(10);
		}

		public static string Description()
		{
			return String(20);
		}

		public static string String(int suggestedLength)
		{
			double q = 1.0 - (1.0 / suggestedLength);
			StringBuilder builder = new StringBuilder();
			do
			{
				builder.Append(Character());
			} while (_rnd.NextDouble() < q);
			return builder.ToString();
		}

		public static string Value(IListValueProperty property)
		{
			var values = property.AllValues;
			if (values.Length == 0)
				return null;
			return values[_rnd.Next(values.Length)];
		}

		public static double Estimate()
		{
			return _rnd.NextDouble() * 100.0;
		}
	}
}