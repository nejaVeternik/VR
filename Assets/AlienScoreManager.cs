using UnityEngine;
using TMPro;

public class AlienScoreManager : MonoBehaviour
{
    public static AlienScoreManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;  // Reference to the TextMeshProUGUI element to display the score
    private int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddPoints(int points)
    {
        score += points;
        Debug.Log($"Score increased by {points}. Current score: {score}");
        UpdateScoreText();
    }

    public void ReducePoints(int points)
    {
        score -= points;
        if (score < 0)
        {
            score = 0;
        }
        Debug.Log($"Score decreased by {points}. Current score: {score}");
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public int GetScore()
    {
        return score;
    }
}
