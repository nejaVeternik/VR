using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class ButtonsGame : MonoBehaviour
{
    private Transform currentParent;
    private PokeInteractable button;
    public int basePoints = 10; 
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
        if (Buttons.Instance != null)
        {
            Buttons.Instance.OnLightPressed(light);
        }
    }
}
