using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Alien : MonoBehaviour
{
    private PokeInteractable alien;
    public int basePoints = 10;               // Default points for normal objects
    public int extraPoints = 0;

    void Awake()
    {
        alien = GetComponentInChildren<PokeInteractable>();
        if (alien == null)
        {
            Debug.LogError("No PokeInteractable found on the GameObject or its children!");
        }

    }

    void Start()
    {
        alien.WhenInteractorAdded.Action += OnPoked;
    }

    void OnDestroy()
    {
        alien.WhenInteractorAdded.Action -= OnPoked;
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (Buttons.Instance != null && Buttons.Instance.IsCurrentLight(GetComponent<Light>()))
        {
            if (AlienScoreManager.Instance != null)
            {
                AlienScoreManager.Instance.AddPoints(basePoints + extraPoints);  // Add points to the score
            }

            GetComponent<Light>().enabled = false; // Disable the Light component

            Buttons.Instance.LightUpRandomLight(); // Select the next random light
        }

        Debug.Log("Poked the correct light!");
    }
}
