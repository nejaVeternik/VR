using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Oculus.Interaction;
using static Oculus.Interaction.InteractableColorVisual;

public class StopGoGame : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public UnityEvent OnGo, OnStop;
    public InteractableColorVisual plate1Visual;
    public InteractableColorVisual plate2Visual;
    public PokeInteractable plate1PokeInteractable;
    public PokeInteractable plate2PokeInteractable;
    public TextMeshProUGUI ResultPlate1;
    public TextMeshProUGUI ResultPlate2;
    public TextMeshProUGUI scoreText;

    private float timer;
    private float timeForNextChange;
    private bool isGoCommand;
    public float minGoDuration = 3f;
    public float maxGoDuration = 7f;
    public float minStopDuration = 1f;
    public float maxStopDuration = 3f;

    private ColorState green = new ColorState() { Color = Color.green };
    private ColorState red = new ColorState() { Color = Color.red };
    private int plate1GoCount = 0;
    private int plate1StopCount = 0;
    private int plate2GoCount = 0;
    private int plate2StopCount = 0;
    private int score = 0;
    

    void Start()
    {
        isGoCommand = false;
        SetRandomTimeForNextChange();
        UpdateText();

        plate1PokeInteractable.WhenInteractorAdded.Action += (pokeInteractor) => OnPlatePoked(ref plate1GoCount, ref plate1StopCount, ResultPlate1);
        plate2PokeInteractable.WhenInteractorAdded.Action += (pokeInteractor) => OnPlatePoked(ref plate2GoCount, ref plate2StopCount, ResultPlate2);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeForNextChange)
        {
            isGoCommand = !isGoCommand;
            SetRandomTimeForNextChange();
            UpdateText();

            if (isGoCommand)
            {
                OnGo.Invoke();
            }
            else
            {
                OnStop.Invoke();
            }
        }
    }

    void SetRandomTimeForNextChange()
    {
        timeForNextChange = isGoCommand ? Random.Range(minGoDuration, maxGoDuration) : Random.Range(minStopDuration, maxStopDuration);
        timer = 0f;
    }

    void UpdateText()
    {
        if (isGoCommand)
        {
            textField.text = "GO";
            textField.color = Color.green; 
            UpdatePlateColors(green);
        }
        else
        {
            textField.text = "STOP";
            textField.color = Color.red; 
            UpdatePlateColors(red);
        }
    }

    void UpdatePlateColors(ColorState hoverColor)
    {
        if(plate1Visual != null)
            plate1Visual.InjectOptionalSelectColorState(hoverColor);

        if(plate2Visual != null)
            plate2Visual.InjectOptionalSelectColorState(hoverColor);
    }

    private void OnPlatePoked(ref int goCount, ref int stopCount, TextMeshProUGUI resultText)
    {
        if (isGoCommand)
        {
            goCount++;
            score += 10;
        }
        else
        {
            stopCount++;
            score -= 50;
        }
        UpdateResultText(resultText, goCount, stopCount);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateResultText(TextMeshProUGUI textMesh, int goCount, int stopCount)
    {
        string goText = $"<color=#00FF00> {goCount}</color>"; 
        string stopText = $"<color=#FF0000> {stopCount}</color>";
        textMesh.text = $"{goText} : {stopText}";
    }

}