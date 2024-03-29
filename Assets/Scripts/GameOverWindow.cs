using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverWindow : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    public Button retryButton;

    public GameObject GameOver;

    private void Awake()
    {
        scoreText = GameOver.transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        retryButton = GameOver.transform.Find("retryButton").GetComponent<Button>();

        retryButton.onClick.AddListener(ButtonClick);
        Hide();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Debug.Log("I got added to OnDied");
    }

    private void Bird_OnDied (object sender, System.EventArgs e)
    {
        Debug.Log("BIRD DEAD in WINDOW");
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        Show();

    }

    public void ButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void Hide()
    {
        GameOver.SetActive(false);
    }

    private void Show()
    {
        GameOver.SetActive(true);
    }
}
