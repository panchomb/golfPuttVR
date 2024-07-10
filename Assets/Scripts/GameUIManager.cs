using UnityEngine;
using UnityEngine.SceneManagement;
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
        shotNumber++;
        shotNumberText.text = "Tiro #: " + shotNumber;
    }

    public void UpdateScore()
    {
        score++;
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

    public void ToggleTerrainTexture()
    {
        GolfTerrainGenerator terrainGenerator = FindObjectOfType<GolfTerrainGenerator>();
        if (terrainGenerator != null)
        {
            terrainGenerator.ToggleHeightmap();
        }
    }

    public void RestartGame()
    {
        shotNumber = 1;
        score = 0;
        timer = 0f;
        isTiming = true;

        shotNumberText.text = "Tiro #: " + shotNumber;
        scoreText.text = "Resultado: " + score;
        UpdateTimerText();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
