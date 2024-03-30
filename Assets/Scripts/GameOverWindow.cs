using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class GameOverWindow : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button retryButton;
    public Button MainMenuButton;
    public TextMeshProUGUI BestScore;

    public GameObject GameOver;

    public int Current = 0;

    public static int NewHigh = 0;

    private void Awake()
    {
        scoreText = GameOver.transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        retryButton = GameOver.transform.Find("retryButton").GetComponent<Button>();
        BestScore = GameOver.transform.Find("HighScore").GetComponent<TextMeshProUGUI>();

        retryButton.onClick.AddListener(ButtonClick);
        MainMenuButton.onClick.AddListener(MainMenuClick);
        HideOver();
    }

    private void Start()
    {
        //Bird.GetInstance().OnDied += Bird_OnDied;
        Debug.Log("I got added to OnDied");
    }

    private void Bird_OnDied (object sender, System.EventArgs e)
    {
        Debug.Log("BIRD DEAD in WINDOW");
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        CompareScore();
        Show();

    }

    public void CompareScore()
    {
        Current = Level.GetInstance().GetPipesPassedCount();
        if (Current > NewHigh)
        {
            BestScore.text = Current.ToString();
            NewHigh = Current;
        }
        else if (Current < NewHigh)
        {
            Debug.Log("No Change");
            BestScore.text = NewHigh.ToString(); 
        }
    }

    public void ButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void MainMenuClick()
    {
        HideOver();
        ActiveMenu();
    }

    private void HideOver()
    {
        GameOver.SetActive(false);
    }

    private void Show()
    {
        GameOver.SetActive(true);
    }

    private void ActiveMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
