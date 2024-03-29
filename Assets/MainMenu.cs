using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button Quit;
    public Button Play;

    private void Awake()
    {
        Play.onClick.AddListener(PlayClick);
        Quit.onClick.AddListener(QuitClick);
    }

    public void PlayClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void QuitClick()
    {
        Application.Quit();
    }
}
