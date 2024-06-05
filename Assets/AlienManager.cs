using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public GameObject[] aliens;  // Array of alien gameObjects
    public float initialY = 3.7f;  // Initial Y position for aliens when they are down
    public float targetY = 3.84f;  // Target Y position for aliens when they move up
    public float popUpTime = 0.5f;  // Time taken for the alien to pop up
    public float stayUpTime = 2.0f;  // Time the alien stays up

    private int currentAlienIndex = -1;  // Index of the currently active alien

    void Start()
    {
        foreach (GameObject alien in aliens)
        {
            Alien alienScript = alien.GetComponent<Alien>();
            if (alienScript != null)
            {
                alienScript.OnAlienPoked += OnAlienPoked;
                // Initialize the aliens to their initial positions
                Vector3 initialPosition = alien.transform.position;
                Debug.Log($"Alien {alien.name} initial position: {initialPosition}");
                //alien.transform.position = new Vector3(initialPosition.x, initialY, initialPosition.z);
                //Debug.Log($"Alien {alien.name} transformed position: {alien.transform.position}");
            }
        }
        StartCoroutine(PopUpRandomAlien());
    }

    private void OnAlienPoked(GameObject alien)
    {
        StartCoroutine(MoveAlienDown(alien));  // Immediately move the poked alien down
    }

    IEnumerator PopUpRandomAlien()
    {
        while (true)
        {
            // Choose a random alien index different from the current one
            int nextAlienIndex = Random.Range(0, aliens.Length);
            while (nextAlienIndex == currentAlienIndex)
            {
                nextAlienIndex = Random.Range(0, aliens.Length);
            }

            // Move the chosen alien up
            currentAlienIndex = nextAlienIndex;
            GameObject alien = aliens[currentAlienIndex];
            Vector3 initialPosition = new Vector3(alien.transform.position.x, alien.transform.position.y, alien.transform.position.z);
            Vector3 targetPosition = new Vector3(alien.transform.position.x, 3.84f, alien.transform.position.z);

            yield return StartCoroutine(MoveAlienUp(alien, initialPosition, targetPosition));

            // Wait for a certain time before moving the alien down
            yield return new WaitForSeconds(stayUpTime);

            yield return StartCoroutine(MoveAlienDown(alien));
        }
    }

    IEnumerator MoveAlienUp(GameObject alien, Vector3 initialPosition, Vector3 targetPosition)
    {
        float timer = 0;
        while (timer < popUpTime)
        {
            alien.transform.position = Vector3.Lerp(initialPosition, targetPosition, timer / popUpTime);
            timer += Time.deltaTime;
            yield return null;
        }
        alien.transform.position = targetPosition;
        Debug.Log($"{alien.name} moved up.");
    }

    IEnumerator MoveAlienDown(GameObject alien)
    {
        Vector3 initialPosition = alien.transform.position;
        Vector3 targetPosition = new Vector3(alien.transform.position.x, 3.7f, alien.transform.position.z);

        float timer = 0;
        while (timer < popUpTime)
        {
            alien.transform.position = Vector3.Lerp(initialPosition, targetPosition, timer / popUpTime);
            timer += Time.deltaTime;
            yield return null;
        }
        alien.transform.position = targetPosition;
        Debug.Log($"{alien.name} moved down.");
    }
}
