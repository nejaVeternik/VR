using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Alien : MonoBehaviour
{
    private PokeInteractable alien;
    public int basePoints = 10;  // Default points for normal objects
    public int extraPoints = 0;

    public delegate void AlienPokedHandler(GameObject alien);
    public event AlienPokedHandler OnAlienPoked;

    private ControllerManager controllerManager;

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
        controllerManager = FindObjectOfType<ControllerManager>();
        alien.WhenInteractorAdded.Action += OnPoked;
    }

    void OnDestroy()
    {
        alien.WhenInteractorAdded.Action -= OnPoked;
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (controllerManager.IsPaused()) return;

        AlienScoreManager.Instance?.AddPoints(basePoints + extraPoints);  // Add points to the score

        Debug.Log("Poked the alien!");

        // Trigger the OnAlienPoked event to inform the manager
        OnAlienPoked?.Invoke(gameObject);
    }
}
