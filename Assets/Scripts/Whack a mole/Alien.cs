using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Alien : MonoBehaviour
{
    private PokeInteractable alien;
    public int basePoints = 10;  
    public int extraPoints = 0;

    public delegate void AlienPokedHandler(GameObject alien, float reactionTime);
    public event AlienPokedHandler OnAlienPoked;

    private ControllerManager controllerManager;
    public float PopUpTime { get; set; }

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

        float reactionTime = Time.time - PopUpTime;
        Debug.Log($"Reaction Time: {reactionTime}");

        OnAlienPoked?.Invoke(gameObject, reactionTime);
    }
}
