using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Interaction;
using static Oculus.Interaction.InteractableColorVisual;

public class platesGame : MonoBehaviour
{
    public List<InteractableColorVisual> plateVisuals; // List of plate visuals
    public List<PokeInteractable> plateInteractables; // List of plate interactables
    public List<TextMeshProUGUI> plateTexts; // List of text components for each plate
    public TextMeshProUGUI scoreText;
    public float timeToPress = 5f; // Time to press the plate

    private float timer;
    private float pressTimer;
    private int currentActivePlate = -1;
    private bool platePressedInTime;
    private int score = 0;

    private ColorState green = new ColorState() { Color = Color.blue };
    private string[] activeText = {"GO", "GO"};

    void Start()
    {
        foreach (var plate in plateInteractables)
        {
            plate.WhenInteractorAdded.Action += OnPlatePoked; // Add OnPlatePoked to all plates
        }

        foreach (var text in plateTexts)
        {
            text.text = ""; // Initialize all texts as empty
        }
    }

    void Update()
    {
        if (currentActivePlate == -1)
        {
            timer += Time.deltaTime;
            // if (timer >= Random.Range(1f, 3f)) // Random time before a plate is activated
            // {
            //     ActivateRandomPlate();
            // }
            ActivateRandomPlate();
        }
        else
        {
            pressTimer += Time.deltaTime;
            if (pressTimer > timeToPress)
            {
                platePressedInTime = false;
                ResetPlates();
            }
        }
    }

    void ActivateRandomPlate()
    {
        currentActivePlate = Random.Range(0, plateVisuals.Count);
        var currentRandomText = Random.Range(0, activeText.Length);
        plateTexts[currentActivePlate].text = activeText[currentRandomText]; // Set the text for the activated plate
        platePressedInTime = false;
        pressTimer = 0f;
    }

    void ResetPlates()
    {
        if (currentActivePlate != -1)
        {
            plateTexts[currentActivePlate].text = ""; // Clear or disable the text
            currentActivePlate = -1;
        }
        timer = 0f;
    }

    private void OnPlatePoked(PokeInteractor pokeInteractor)
    {
        // Determine which plate was poked
        int pokedPlateIndex = plateInteractables.IndexOf(pokeInteractor.GetComponent<PokeInteractable>());

        // if (pokedPlateIndex == currentActivePlate)
        // {
        //     platePressedInTime = true;
        //     score += 100; // Reward for pressing the active plate in time
        //     UpdateScore();
        //     ResetPlates();
        // }
        platePressedInTime = true;
        score += 100; // Reward for pressing the active plate in time
        UpdateScore();
        ResetPlates();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
