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
            EditorSceneManager.OpenScene(Constants.PersistentScenePath, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.GameMainScenePath, OpenSceneMode.Additive);
        }

        [MenuItem("Homicide/Load Main Menu Scenes", priority = 10)]
        public static void LoadMainMenuScenes()
        {
            EditorSceneManager.OpenScene(Constants.PersistentScenePath, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.MainmenuScenePath, OpenSceneMode.Additive);
        }
    }
}
