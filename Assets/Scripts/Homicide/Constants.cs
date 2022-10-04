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
		public static readonly string DATA_PATH = Application.dataPath;

		/// <summary>
		/// Path to the persistent data folder.
		/// </summary>
		public static readonly string PERSISTENT_PATH = Application.persistentDataPath;

		/// <summary>
		/// Path to the temporary cache folder.
		/// </summary>
		public static readonly string CACHE_PATH = Application.temporaryCachePath;

		/// <summary>
		/// Path to the streaming assets folder.
		/// </summary>
		public static readonly string STREAMING_ASSETS = Application.streamingAssetsPath;

		/// <summary>
		/// The name of the persistent scene
		/// </summary>
		public static readonly string PERSISTENT_SCENE_PATH = "Assets/Scenes/Persistent.unity";

		/// <summary>
		/// The name of the main game scene
		/// </summary>
		public const string GAME_MAIN_SCENE_PATH = "Assets/Scenes/GameMain.unity";

		/// <summary>
		/// The name of the start scene
		/// </summary>
		public static readonly string MAINMENU_SCENE_PATH = "Assets/Scenes/MainMenu.unity";
	}
}
