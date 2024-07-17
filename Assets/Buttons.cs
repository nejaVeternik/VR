using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public static Buttons Instance;

    public LightEntry[] lights;
    public float diameter = 1.8f; // Diameter of the torus
    public float minY = 0f; // Minimum Y bound for level 3 lights
    public float maxY = 1.5f; // Maximum Y bound for level 3 lights
    public float blinkInterval = 0.5f;
    public float moveSpeed = 0.05f; // Speed at which the lights move
    public float circleRadius = 0.1f; // Radius of the circular motion, reduced for smaller circles
    private Light currentLight;
    private float timer;
    private bool isLightOn;
    private ControllerManager controllerManager;
    private string currentColor; // Track the current color for level 2
    private List<LightEntry> activeLights = new List<LightEntry>(); // Track active lights for level 2 and 3
    private Dictionary<GameObject, float> circleOffsets = new Dictionary<GameObject, float>(); // Track offsets for circular motion
    private Dictionary<GameObject, bool> movementType = new Dictionary<GameObject, bool>(); // Track movement type (true for circular, false for up and down)
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>(); // Track initial positions for circular motion
    private bool isGameFinished = false; // Flag to stop lighting up lights
    private Dictionary<Light, float> lightUpTimes = new Dictionary<Light, float>(); // Track the time when each light is lit up
    private Dictionary<Light, float> pressTimes = new Dictionary<Light, float>(); // Track the time when each light is pressed
    private float lastPressTime; // Track the time of the last press in level 2

    private int currentLevel = 1;
    private int maxLevel = 3;

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
        controllerManager = FindObjectOfType<ControllerManager>();

        if (lights.Length != 26)
        {
            Debug.LogError("The lights array must contain exactly 26 LightEntry instances.");
            return;
        }

        // Ensure all parent objects are active at the start
        foreach (LightEntry entry in lights)
        {
            entry.parentObject.SetActive(true);
            entry.light.enabled = false; // Ensure all lights are initially turned off
            initialPositions[entry.parentObject] = entry.parentObject.transform.position; // Store the initial position
        }

        LightUpRandomLight();
    }

    void Update()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isGameFinished) return; // Do nothing if the game is finished

        if (currentLevel == 1 && currentLight != null)
        {
            timer += Time.deltaTime;
            if (timer >= blinkInterval)
            {
                isLightOn = !isLightOn;
                currentLight.enabled = isLightOn;
                timer = 0f;
            }
        }
        else if (currentLevel == 3)
        {
            MoveActiveLightGameObjects();
        }
    }

    private void MoveActiveLightGameObjects()
    {
        foreach (var entry in activeLights)
        {
            var obj = entry.parentObject; // Move the parent object
            if (!circleOffsets.ContainsKey(obj))
            {
                circleOffsets[obj] = Random.Range(0f, 2f * Mathf.PI);
                movementType[obj] = Random.value > 0.5f; // Randomly choose the movement type
            }

            float offset = circleOffsets[obj];
            Vector3 initialPosition = initialPositions[obj]; // Get the initial position

            Vector3 newPosition = initialPosition;

            if (movementType[obj]) // Circular motion in the y-z plane
            {
                newPosition.y += Mathf.Cos(Time.time * (moveSpeed * 8) + offset) * circleRadius;
                newPosition.z += Mathf.Sin(Time.time * (moveSpeed * 8) + offset) * circleRadius;
            }
            else // Move up and down
            {
                float amplitude = 0.1f; // Amplitude of 0.1 units for a total bounce height of 0.2 units
                newPosition.y = initialPosition.y + Mathf.PingPong(Time.time * moveSpeed, amplitude * 2f) - amplitude;
            }

            obj.transform.position = newPosition;
        }
    }

    public void LightUpRandomLight()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isGameFinished) return; // Do nothing if the game is finished

        foreach (LightEntry entry in lights)
        {
            entry.light.enabled = false;
        }

        if (currentLevel == 1)
        {
            int randomIndex = Random.Range(0, lights.Length);
            currentLight = lights[randomIndex].light;
            currentLight.enabled = true;
            isLightOn = true;
            timer = 0f;
            lightUpTimes[currentLight] = Time.time; // Record the time when the light is lit up
        }
        else if (currentLevel == 2)
        {
            int randomIndex = Random.Range(0, lights.Length);
            currentColor = lights[randomIndex].color;

            activeLights.Clear();
            foreach (LightEntry entry in lights)
            {
                if (entry.color == currentColor)
                {
                    entry.light.enabled = true;
                    activeLights.Add(entry);
                    lightUpTimes[entry.light] = Time.time; // Record the time when each light is lit up
                }
            }
            lastPressTime = lightUpTimes[activeLights[0].light]; // Initialize the last press time with the first light's up time
        }
        else if (currentLevel == 3)
        {
            // Enable all parent objects (they should already be enabled, but ensure they are)
            foreach (LightEntry entry in lights)
            {
                entry.parentObject.SetActive(true);
            }

            // Light up a random light from lights
            int randomIndex = Random.Range(0, lights.Length);
            currentLight = lights[randomIndex].light;
            currentLight.enabled = true;
            isLightOn = true;
            timer = 0f;
            lightUpTimes[currentLight] = Time.time; // Record the time when the light is lit up

            // Track the active light
            activeLights.Clear();
            activeLights.Add(lights[randomIndex]);
        }
    }

    public void OnLightPressed(Light pressedLight)
    {
        if (!lightUpTimes.ContainsKey(pressedLight))
        {
            Debug.LogWarning("Pressed light not found in lightUpTimes dictionary.");
            return;
        }

        float reactionTime = Time.time - (currentLevel == 2 ? lastPressTime : lightUpTimes[pressedLight]); // Calculate reaction time based on level
        TrackerController.Instance.RecordReactionTime(reactionTime); // Send reaction time to controller

        if (currentLevel == 2)
        {
            pressTimes[pressedLight] = Time.time; // Record the press time for level 2
        }

        if (currentLevel == 2)
        {
            LightEntry pressedEntry = activeLights.Find(entry => entry.light == pressedLight);
            if (pressedEntry != null)
            {
                pressedLight.enabled = false;
                activeLights.Remove(pressedEntry);

                if (activeLights.Count == 0)
                {
                    // Calculate the average reaction time for level 2
                    float totalReactionTime = 0f;
                    foreach (var pressTime in pressTimes)
                    {
                        totalReactionTime += pressTime.Value - lightUpTimes[pressTime.Key];
                    }
                    float averageReactionTime = totalReactionTime / pressTimes.Count;

                    // Special scoring for level 2 based on average reaction time
                    int level2Points = CalculatePoints(averageReactionTime);
                    ScoreManagerGait.Instance.AddPoints(level2Points);
                    ScoreManagerGait.Instance.IncrementGroupsPressedInLevel2(); // Track groups pressed

                    // Clear pressTimes and lightUpTimes for next group
                    pressTimes.Clear();
                    lightUpTimes.Clear();

                    LightUpRandomLight(); // Light up another set of lights
                }
                else
                {
                    lastPressTime = Time.time; // Reset the last press time
                }
            }
        }
        else if (currentLevel == 1 && IsCurrentLight(pressedLight))
        {
            // Handle level 1 logic
            int points = CalculatePoints(reactionTime);
            pressedLight.enabled = false;
            ScoreManagerGait.Instance.AddPoints(points); // Add points based on reaction time
            ScoreManagerGait.Instance.IncrementLightsPressedInLevel1();
            LightUpRandomLight();
        }
        else if (currentLevel == 3)
        {
            LightEntry pressedEntry = activeLights.Find(entry => entry.light == pressedLight);
            if (pressedEntry != null)
            {
                int points = CalculatePoints(reactionTime);
                pressedLight.enabled = false;
                activeLights.Remove(pressedEntry);
                ScoreManagerGait.Instance.AddPoints(points); // Add points based on reaction time
                ScoreManagerGait.Instance.IncrementLightsPressedInLevel3();
                LightUpRandomLight();
            }
        }
    }

    private int CalculatePoints(float reactionTime)
    {
        if (reactionTime < 5f)
        {
            return 20;
        }
        else if (reactionTime < 10f)
        {
            return 10;
        }
        else
        {
            return 5;
        }
    }

    public bool IsCurrentLight(Light light)
    {
        return light == currentLight;
    }

    public void NextLevel()
    {
        Debug.Log("Next level");
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            LightUpRandomLight();
        }
        else
        {
            Debug.Log("Max level reached!");
        }
    }

    public void StopLightingUp()
    {
        isGameFinished = true;
        // Disable all lights
        foreach (LightEntry entry in lights)
        {
            entry.light.enabled = false;
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}

[System.Serializable]
public class LightEntry
{
    public Light light;
    public GameObject parentObject; // Reference to the parent game object
    public string color;
}
