using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    public void loadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
