using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public TMP_Text shotNumberText;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    private int shotNumber = 1;
    private int score = 0;
    private float timer = 0f;
    private bool isTiming = true;

    void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void UpdateShotNumber()
    {
        shotNumber = shotNumber + 1;
        shotNumberText.text = "Tiro #: " + shotNumber;
    }

    public void UpdateScore()
    {
        score = score + 1;
        scoreText.text = "Resultado: " + score;
    }

    public void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string timeString = string.Format("{0:0}m {1:00}s", minutes, seconds);
        timeText.text = "Tiempo: " + timeString;
    }

    public void StopTimer()
    {
        isTiming = false;
    }

    public void StartTimer()
    {
        timer = 0f;
        isTiming = true;
    }
}
