using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public static Buttons Instance;

    public Light[] lights;
    public float blinkInterval = 0.5f;
    private Light currentLight;
    private float timer;
    private bool isLightOn;

    void Awake()
    {
        // Ensure there is only one instance of the LightManager
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
        if (lights.Length != 26)
        {
            Debug.LogError("The lights array must contain exactly 26 Light instances.");
            return;
        }

        LightUpRandomLight();
    }

    void Update()
    {
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