using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button Quit;
    public Button Play;
    public Button AIPlay;

    private void Awake()
    {
        Play.onClick.AddListener(PlayClick);
        AIPlay.onClick.AddListener(AIPlayClick);
        Quit.onClick.AddListener(QuitClick);

    }

    public void PlayClick()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void AIPlayClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("AIGameScene");
    }

    public void QuitClick()
    {
        Application.Quit();
    }
}
