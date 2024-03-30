using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreWindow : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI highText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        highText = transform.Find("HighScoreNum").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        highText.text = GameOverWindow.NewHigh.ToString();
    }
}
