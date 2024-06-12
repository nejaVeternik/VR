using UnityEngine;
using TMPro;  // Add this to access TextMeshPro components

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;  // Singleton instance of the Score Manager

    public TextMeshProUGUI scoreText;     // Reference to the TextMeshProUGUI component
    public int Score { get; private set; }  // Current score

    private void Awake()
    {
        // Ensure that there is only one instance of the Score Manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the score text if it's assigned
        if (scoreText != null)
            scoreText.text = "" + 0;
    }

    // Method to add points to the score
    public void AddPoints(int points)
    {
        Score += points;  // Update the score
        
        // Update the TextMeshPro text
        if (scoreText != null)
            scoreText.text = Score.ToString();
    }
}
