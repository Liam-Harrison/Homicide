using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Homicide.Editor
{
    public class EditorSceneLoader : MonoBehaviour
    {
        [MenuItem("Homicide/Load Game Scenes", priority = 20)]
        public static void LoadGameScenes()
        {
            EditorSceneManager.OpenScene(Constants.PERSISTENT_SCENE_PATH, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.GAME_MAIN_SCENE_PATH, OpenSceneMode.Additive);
        }

        [MenuItem("Homicide/Load Main Menu Scenes", priority = 10)]
        public static void LoadMainMenuScenes()
        {
            EditorSceneManager.OpenScene(Constants.PERSISTENT_SCENE_PATH, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.MAINMENU_SCENE_PATH, OpenSceneMode.Additive);
        }
    }
}
