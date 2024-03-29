using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverWindow : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    public Button retryButton;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        retryButton = transform.Find("retryButton").GetComponent<Button>();

        retryButton.onClick.AddListener(ButtonClick);
        Hide();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        Debug.Log("BIRD DEAD in WINDOW");
        Show();
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
    }

    public void ButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
