using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public GameObject[] aliens;  // Array of alien gameObjects
    public float popUpHeight = 1.0f;  // Height the alien will move up
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
            Vector3 initialPosition = alien.transform.position;
            Vector3 targetPosition = initialPosition + Vector3.up * popUpHeight;

            yield return StartCoroutine(MoveAlienUp(alien, initialPosition, targetPosition));

            alien.SetActive(true);

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
        Vector3 targetPosition = alien.transform.position - Vector3.up * popUpHeight;
        Vector3 initialPosition = alien.transform.position;

        float timer = 0;
        while (timer < popUpTime)
        {
            alien.transform.position = Vector3.Lerp(initialPosition, targetPosition, timer / popUpTime);
            timer += Time.deltaTime;
            yield return null;
        }
        alien.transform.position = targetPosition;
        alien.SetActive(false);
        Debug.Log($"{alien.name} moved down.");
    }
}
