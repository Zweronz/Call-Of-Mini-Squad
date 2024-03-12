using System.Collections;

namespace ChartboostSDK
{
	public sealed class CBLocation
	{
		private readonly string name;

		private static Hashtable map = new Hashtable();

		public static readonly CBLocation Default = new CBLocation("Default");

		public static readonly CBLocation Startup = new CBLocation("Startup");

		public static readonly CBLocation HomeScreen = new CBLocation("Home Screen");

		public static readonly CBLocation MainMenu = new CBLocation("Main Menu");

		public static readonly CBLocation GameScreen = new CBLocation("Game Screen");

		public static readonly CBLocation Achievements = new CBLocation("Achievements");

		public static readonly CBLocation Quests = new CBLocation("Quests");

		public static readonly CBLocation Pause = new CBLocation("Pause");

		public static readonly CBLocation LevelStart = new CBLocation("Level Start");

		public static readonly CBLocation LevelComplete = new CBLocation("Level Complete");

		public static readonly CBLocation TurnComplete = new CBLocation("Turn Complete");

		public static readonly CBLocation IAPStore = new CBLocation("IAP Store");

		public static readonly CBLocation ItemStore = new CBLocation("Item Store");

		public static readonly CBLocation GameOver = new CBLocation("Game Over");

		public static readonly CBLocation LeaderBoard = new CBLocation("Leaderboard");

		public static readonly CBLocation Settings = new CBLocation("Settings");

		public static readonly CBLocation Quit = new CBLocation("Quit");

		private CBLocation(string name)
		{
			this.name = name;
			map.Add(name, this);
		}

		public override string ToString()
		{
			return name;
		}

		public static CBLocation locationFromName(string name)
		{
			if (name == null)
			{
				return Default;
			}
			if (map[name] != null)
			{
				return map[name] as CBLocation;
			}
			return new CBLocation(name);
		}
	}
}
