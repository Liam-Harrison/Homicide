using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Homicide
{
    /// <summary>
    /// This class allows for other scripts or components in the scene to load game scenes.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [Flags]
        private enum Scene
        {
            Persistent = 1 << 1,
            MainMenu = 1 << 2,
            GameMain = 1 << 3,
        }

        [SerializeField, Title("Settings")]
        private Scene scene;

        [SerializeField]
        private LoadSceneMode mode;

        [SerializeField, InfoBox("Load on awake only works in builds, does not occur in editor. Destroys component afterwards.", visibleIfMemberName: nameof(loadOnAwake))]
        private bool loadOnAwake;

        private void Awake()
        {
            if (loadOnAwake)
            {
#if !UNITY_EDITOR
			    LoadScene();
#endif
                Destroy(this);
            }
        }

        /// <summary>
        /// Load the scenes setup in this component according to its scene flag settings.
        /// </summary>
        public void LoadScene()
        {
            var loaded = false;

            Time.timeScale = 1;

            if (scene.HasFlag(Scene.GameMain))
            {
                SceneManager.LoadSceneAsync(Constants.GAME_MAIN_SCENE_PATH, mode);
                loaded = true;
            }

            if (scene.HasFlag(Scene.Persistent))
            {
                SceneManager.LoadSceneAsync(Constants.PERSISTENT_SCENE_PATH, loaded ? LoadSceneMode.Additive : mode);
                loaded = true;
            }

            if (scene.HasFlag(Scene.MainMenu))
            {
                SceneManager.LoadSceneAsync(Constants.MAINMENU_SCENE_PATH, loaded ? LoadSceneMode.Additive : mode);
            }
        }
    }
}