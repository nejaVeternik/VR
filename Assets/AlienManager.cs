using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public GameObject[] aliens = new GameObject[8];  // Array of alien gameObjects
    public float popUpHeight = 1.0f;  // Height the alien will move up
    public float popUpTime = 0.5f;  // Time taken for the alien to pop up
    public float stayUpTime = 2.0f;  // Time the alien stays up

    private int currentAlienIndex = -1;  // Index of the currently active alien

    void Start()
    {
        StartCoroutine(PopUpRandomAlien());
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

            float timer = 0;
            while (timer < popUpTime)
            {
                alien.transform.position = Vector3.Lerp(initialPosition, targetPosition, timer / popUpTime);
                timer += Time.deltaTime;
                yield return null;
            }
            alien.transform.position = targetPosition;

            alien.SetActive(true);

            // Wait for a certain time before moving the alien down
            yield return new WaitForSeconds(stayUpTime);

            // Move the alien back down
            timer = 0;
            while (timer < popUpTime)
            {
                alien.transform.position = Vector3.Lerp(targetPosition, initialPosition, timer / popUpTime);
                timer += Time.deltaTime;
                yield return null;
            }
            alien.transform.position = initialPosition;

            alien.SetActive(false);

            // Wait for a short period before the next alien pops up
            yield return new WaitForSeconds(1.0f);
        }
    }
}
