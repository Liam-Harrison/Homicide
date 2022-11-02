using UnityEngine;

namespace Homicide
{
	/// <summary>
	/// Contains a collection of common constants that can be used throughout the game.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// Path to the game data folder.
		/// </summary>
		public static readonly string DataPath = Application.dataPath;

		/// <summary>
		/// Path to the persistent data folder.
		/// </summary>
		public static readonly string PersistentPath = Application.persistentDataPath;

		/// <summary>
		/// Path to the temporary cache folder.
		/// </summary>
		public static readonly string CachePath = Application.temporaryCachePath;

		/// <summary>
		/// Path to the streaming assets folder.
		/// </summary>
		public static readonly string StreamingAssets = Application.streamingAssetsPath;

		/// <summary>
		/// The name of the persistent scene
		/// </summary>
		public static readonly string PersistentScenePath = "Assets/Scenes/Persistent.unity";

		/// <summary>
		/// The name of the main game scene
		/// </summary>
		public const string GameMainScenePath = "Assets/Scenes/GameMain.unity";

		/// <summary>
		/// The name of the start scene
		/// </summary>
		public static readonly string MainmenuScenePath = "Assets/Scenes/MainMenu.unity";
	}

	public enum Weapon
	{
		None,
		Knife,
	}
}
