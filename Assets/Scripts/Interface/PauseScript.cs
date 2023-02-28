using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    bool PauseGame = true;
    public GameObject pauseGameMenu;

    public void Stop()
    {
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
    }

    public void BackClick()
    {
        SceneManager.LoadScene(0);
    }

    public void ResumeClick()
    {
        pauseGameMenu.SetActive(false);
    }
}
