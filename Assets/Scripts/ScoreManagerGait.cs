using UnityEngine;
using TMPro;

public class ScoreManagerGait : MonoBehaviour
{
    public static ScoreManagerGait Instance;  // Singleton instance of the Score Manager

    public TextMeshProUGUI scoreText;  // Reference to the TextMeshProUGUI component
    public TextMeshProUGUI scoreText1;
    public int Score { get; private set; }  // Current score

    private ControllerManager controllerManager;

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

        if (scoreText1 != null)
            scoreText1.text = "" + 0;
    }

    private void Start()
    {
        controllerManager = FindObjectOfType<ControllerManager>();
    }

    // Method to add points to the score
    public void AddPoints(int points)
    {
        // Check if the game is paused
        if (controllerManager != null && controllerManager.IsPaused()) return;

        Score += points;  // Update the score

        // Update the TextMeshPro text
        if (scoreText != null)
            scoreText.text = Score.ToString();

        if (scoreText1 != null)
            scoreText1.text = Score.ToString();
    }
}
