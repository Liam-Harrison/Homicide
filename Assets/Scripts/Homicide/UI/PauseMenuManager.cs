using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] GameObject pauseCanvas;
    public static bool gameIsPaused = false;
  


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameIsPaused = !gameIsPaused;
            Pause();
        }
    }

    public void loadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    public void Pause()
    {
        if(gameIsPaused)
        {
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true);
        }
        else
        {
            Resume();
        }
    }

    void Resume()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
    }

    public void ResumeOnClick()
    {
        gameIsPaused = !gameIsPaused;
        Resume();
    }


}
