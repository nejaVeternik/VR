using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManagerGait : MonoBehaviour
{
    public static ScoreManagerGait Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelText1;
    public TextMeshProUGUI endGameScoreText;
    public int Score { get; private set; }
    public GameObject gameFinishedMenu; 

    public int lightsToPressForLevel2 = 3; 
    public int groupsToPressForLevel3 = 2; 
    public int lightsToPressToEndGame = 3; 

    private ControllerManager controllerManager;
    private Buttons buttonsInstance;
    private bool isGameFinished = false;

    private int lightsPressedInLevel1 = 0; 
    private int groupsPressedInLevel2 = 0; 
    private int lightsPressedInLevel3 = 0; 

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

        CheckProgression();
    }

    private void CheckProgression()
    {
        if (buttonsInstance.GetCurrentLevel() == 1 && lightsPressedInLevel1 >= lightsToPressForLevel2)
        {
            if (levelText != null) levelText.text = "Nivo 2";
            if (levelText1 != null) levelText1.text = "Nivo 2";
            buttonsInstance.NextLevel();
        }
        else if (buttonsInstance.GetCurrentLevel() == 2 && groupsPressedInLevel2 >= groupsToPressForLevel3)
        {
            if (levelText != null) levelText.text = "Nivo 3";
            if (levelText1 != null) levelText1.text = "Nivo 3";
            buttonsInstance.NextLevel();
        }
        else if (buttonsInstance.GetCurrentLevel() == 3 && lightsPressedInLevel3 >= lightsToPressToEndGame)
        {
            if (!isGameFinished)
            {
                isGameFinished = true; 
                ShowGameFinishedMenu();
            }
        }
    }

    public void IncrementLightsPressedInLevel1()
    {
        lightsPressedInLevel1++;
        CheckProgression();
    }

    public void IncrementGroupsPressedInLevel2()
    {
        groupsPressedInLevel2++;
        CheckProgression();
    }

    public void IncrementLightsPressedInLevel3()
    {
        lightsPressedInLevel3++;
        CheckProgression();
    }

    private void ShowGameFinishedMenu()
    {
        if (controllerManager != null && gameFinishedMenu != null)
        {
            if (endGameScoreText != null)
            {
                endGameScoreText.text = "Igra zakljucena. Rezultat: " + Score.ToString();
            }

            controllerManager.PositionMenuInFrontOfPlayer(gameFinishedMenu);
            gameFinishedMenu.SetActive(true);

            if (buttonsInstance != null)
            {
                buttonsInstance.StopLightingUp();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Gait");
    }
}
