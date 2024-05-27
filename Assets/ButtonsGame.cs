using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class ButtonsGame : MonoBehaviour
{
    private Transform currentParent;
    private PokeInteractable button;
    public int basePoints = 10;               // Default points for normal objects
    public int extraPoints = 0;
    private Light light;

    void Awake()
    {
        button = GetComponentInChildren<PokeInteractable>();
        if (button == null)
        {
            Debug.LogError("No PokeInteractable found on the GameObject or its children!");
        }

        light = GetComponentInChildren<Light>();
        if (light == null)
        {
            Debug.LogError("No Light component found on the GameObject or its children!");
        }
    }

    void Start()
    {
        button.WhenInteractorAdded.Action += OnPoked;
    }

    void OnDestroy()
    {
        button.WhenInteractorAdded.Action -= OnPoked;
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (Buttons.Instance != null && Buttons.Instance.IsCurrentLight(light))
        {
            if (ScoreManagerGait.Instance != null)
            {
                ScoreManagerGait.Instance.AddPoints(basePoints + extraPoints);  // Add points to the score
            }

            light.enabled = false; // Disable the Light component

            Buttons.Instance.LightUpRandomLight(); // Select the next random light
        }

        Debug.Log("Poked the correct light!");
    }
}
