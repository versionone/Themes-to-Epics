using System.Text;

namespace VersionOne.Themes_to_Epics.Tests
{
	public static class Random
	{
		private static readonly System.Random _rnd = new System.Random();
		private const string _characters = "abcdefghijklmnopqrstuvwxyz ABCDDEFHIJKLMNOPQRSTUVWXYZ 0123456789";

		public static char Character()
		{
			return _characters[_rnd.Next(_characters.Length)];
		}

		public static string Name()
		{
			return String(10);
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
	}
}