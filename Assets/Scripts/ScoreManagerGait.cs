using UnityEngine;
using TMPro;

public class ScoreManagerGait : MonoBehaviour
{
    public static ScoreManagerGait Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelText1;
    public int Score { get; private set; }

    private ControllerManager controllerManager;
    private Buttons buttonsInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (scoreText != null)
            scoreText.text = "" + 0;

        if (scoreText1 != null)
            scoreText1.text = "" + 0;
    }

    private void Start()
    {
        controllerManager = FindObjectOfType<ControllerManager>();
        buttonsInstance = Buttons.Instance;
    }

    public void AddPoints(int points)
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;

        Score += points;

        if (scoreText != null)
            scoreText.text = Score.ToString();

        if (scoreText1 != null)
            scoreText1.text = Score.ToString();

        // Trigger next level based on score threshold
        if (Score == 20)
        {
            if (levelText != null) levelText.text = "Nivo 2";
            if (levelText1 != null) levelText1.text = "Nivo 2";
            buttonsInstance.NextLevel();
        }
        else if (Score == 40) // Assuming level 3 starts at 40 points
        {
            if (levelText != null) levelText.text = "Nivo 3";
            if (levelText1 != null) levelText1.text = "Nivo 3";
            buttonsInstance.NextLevel();
        }
    }
}
