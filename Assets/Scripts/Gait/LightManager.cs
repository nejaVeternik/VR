using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    public Light[] lights;
    public float blinkInterval = 0.5f;
    private Light currentLight;
    private float timer;
    private bool isLightOn;
    private ControllerManager controllerManager;

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
            Debug.LogError("The lights array must contain exactly 26 Light instances.");
            return;
        }

        LightUpRandomLight();
    }

    void Update()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;

        if (currentLight != null)
        {
            timer += Time.deltaTime;
            if (timer >= blinkInterval)
            {
                isLightOn = !isLightOn;
                currentLight.enabled = isLightOn;
                timer = 0f;
            }
        }
    }

    public void LightUpRandomLight()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;

        int randomIndex = Random.Range(0, lights.Length);

        foreach (Light light in lights)
        {
            light.enabled = false;
        }

        currentLight = lights[randomIndex];
        currentLight.enabled = true;
        isLightOn = true;
        timer = 0f;
    }

    public bool IsCurrentLight(Light light)
    {
        return light == currentLight;
    }
}
