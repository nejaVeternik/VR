using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;     
    public int Score { get; private set; }
    private bool loggingEnabled = false;
    private int totalPoints;
    private float totalHitTime;
    private float totalHitHeight;
    private int hitCount;
    private bool gameFinished = false;

    private void Awake()
    {
        if (scoreText != null) scoreText.text = "" + 0;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        if (gameFinished) return;

        Score += points;  

        if (scoreText != null) scoreText.text = Score.ToString();

        totalPoints += points;
        if (loggingEnabled) Debug.Log("Total Points: " + totalPoints);
    }

    public void UpdateAverageHitTime(float hitTime)
    {
        totalHitTime += hitTime;
        hitCount++;
        if (loggingEnabled) Debug.Log("Average Hit Time: " + GetAverageHitTime() + " seconds");
    }

    public void UpdateAverageHitHeight(float hitHeight)
    {
        totalHitHeight += hitHeight;
        if (loggingEnabled) Debug.Log("Average Hit Height: " + GetAverageHitHeight() + " meters");
    }

    public float GetAverageHitTime()
    {
        return hitCount > 0 ? totalHitTime / hitCount : 0f;
    }

    public float GetAverageHitHeight()
    {
        return hitCount > 0 ? totalHitHeight / hitCount : 0f;
    }

    public int GetScore()
    {
        return Score;
    }

    public void DisplayGameFinished()
    {
        gameFinished = true;
        
    }
}
