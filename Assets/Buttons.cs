using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public static Buttons Instance;

    public LightEntry[] lights;
    public float diameter = 1.8f; 
    public float minY = 0f; 
    public float maxY = 1.5f; 
    public float blinkInterval = 0.5f;
    public float moveSpeed = 0.05f; 
    public float circleRadius = 0.1f; 
    private Light currentLight;
    private float timer;
    private bool isLightOn;
    private ControllerManager controllerManager;
    private string currentColor; 
    private List<LightEntry> activeLights = new List<LightEntry>();
    private Dictionary<GameObject, float> circleOffsets = new Dictionary<GameObject, float>(); 
    private Dictionary<GameObject, bool> movementType = new Dictionary<GameObject, bool>(); 
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private bool isGameFinished = false; 
    private Dictionary<Light, float> lightUpTimes = new Dictionary<Light, float>(); 
    private Dictionary<Light, float> pressTimes = new Dictionary<Light, float>(); 
    private float lastPressTime; 

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

        foreach (LightEntry entry in lights)
        {
            entry.parentObject.SetActive(true);
            entry.light.enabled = false; 
            initialPositions[entry.parentObject] = entry.parentObject.transform.position; 
        }

        LightUpRandomLight();
    }

    void Update()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isGameFinished) return; 

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
            var obj = entry.parentObject; 
            if (!circleOffsets.ContainsKey(obj))
            {
                circleOffsets[obj] = Random.Range(0f, 2f * Mathf.PI);
                movementType[obj] = Random.value > 0.5f; 
            }

            float offset = circleOffsets[obj];
            Vector3 initialPosition = initialPositions[obj]; 

            Vector3 newPosition = initialPosition;

            if (movementType[obj]) 
            {
                newPosition.y += Mathf.Cos(Time.time * (moveSpeed * 8) + offset) * circleRadius;
                newPosition.z += Mathf.Sin(Time.time * (moveSpeed * 8) + offset) * circleRadius;
            }
            else
            {
                float amplitude = 0.1f; 
                newPosition.y = initialPosition.y + Mathf.PingPong(Time.time * moveSpeed, amplitude * 2f) - amplitude;
            }

            obj.transform.position = newPosition;
        }
    }

    public void LightUpRandomLight()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isGameFinished) return; 

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
            lightUpTimes[currentLight] = Time.time; 
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
                    lightUpTimes[entry.light] = Time.time; 
                }
            }
            lastPressTime = lightUpTimes[activeLights[0].light]; 
        }
        else if (currentLevel == 3)
        {
            foreach (LightEntry entry in lights)
            {
                entry.parentObject.SetActive(true);
            }

            int randomIndex = Random.Range(0, lights.Length);
            currentLight = lights[randomIndex].light;
            currentLight.enabled = true;
            isLightOn = true;
            timer = 0f;
            lightUpTimes[currentLight] = Time.time; 

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

        float reactionTime = Time.time - (currentLevel == 2 ? lastPressTime : lightUpTimes[pressedLight]); 
        TrackerController.Instance.RecordReactionTime(reactionTime);

        if (currentLevel == 2)
        {
            pressTimes[pressedLight] = Time.time; 
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
                    float totalReactionTime = 0f;
                    foreach (var pressTime in pressTimes)
                    {
                        totalReactionTime += pressTime.Value - lightUpTimes[pressTime.Key];
                    }
                    float averageReactionTime = totalReactionTime / pressTimes.Count;

                    int level2Points = CalculatePoints(averageReactionTime);
                    ScoreManagerGait.Instance.AddPoints(level2Points);
                    ScoreManagerGait.Instance.IncrementGroupsPressedInLevel2(); 

                    pressTimes.Clear();
                    lightUpTimes.Clear();

                    LightUpRandomLight(); 
                }
                else
                {
                    lastPressTime = Time.time; 
                }
            }
        }
        else if (currentLevel == 1 && IsCurrentLight(pressedLight))
        {
            int points = CalculatePoints(reactionTime);
            pressedLight.enabled = false;
            ScoreManagerGait.Instance.AddPoints(points); 
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
                ScoreManagerGait.Instance.AddPoints(points); 
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
    public GameObject parentObject;
    public string color;
}
