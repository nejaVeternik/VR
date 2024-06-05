using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public GameObject[] aliens;  // Array of alien gameObjects
    public int maxUpAliens = 3;  // Maximum number of aliens that can be up at the same time
    private float initialY = 3.2f;  // Hardcoded initial Y position for aliens when they are down
    private float targetY = 3.418f;  // Hardcoded target Y position for aliens when they move up
    public float minPopUpTime = 0.5f;  // Minimum time taken for the alien to pop up
    public float maxPopUpTime = 1.5f;  // Maximum time taken for the alien to pop up
    public float minStayUpTime = 1.0f;  // Minimum time the alien stays up
    public float maxStayUpTime = 3.0f;  // Maximum time the alien stays up

    private int currentAlienIndex = -1;  // Index of the currently active alien
    private List<int> upAliens = new List<int>();  // List to keep track of aliens that are up

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
                alien.transform.position = new Vector3(initialPosition.x, initialY, initialPosition.z);
                Debug.Log($"Alien {alien.name} transformed position: {alien.transform.position}");
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
            if (upAliens.Count < maxUpAliens)
            {
                // Choose a random alien index that is not up already
                int nextAlienIndex;
                do
                {
                    nextAlienIndex = Random.Range(0, aliens.Length);
                } while (upAliens.Contains(nextAlienIndex));

                // Move the chosen alien up
                currentAlienIndex = nextAlienIndex;
                GameObject alien = aliens[currentAlienIndex];
                Vector3 initialPosition = new Vector3(alien.transform.position.x, initialY, alien.transform.position.z);
                Vector3 targetPosition = new Vector3(alien.transform.position.x, targetY, alien.transform.position.z);

                upAliens.Add(currentAlienIndex);
                yield return StartCoroutine(MoveAlienUp(alien, initialPosition, targetPosition));

                // Wait for a random time before moving the alien down
                float stayUpTime = Random.Range(minStayUpTime, maxStayUpTime);
                yield return new WaitForSeconds(stayUpTime);

                StartCoroutine(MoveAlienDown(alien));
            }
            yield return new WaitForSeconds(0.1f);  // Small delay before checking again
        }
    }

    IEnumerator MoveAlienUp(GameObject alien, Vector3 initialPosition, Vector3 targetPosition)
    {
        float popUpTime = Random.Range(minPopUpTime, maxPopUpTime);
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
        int alienIndex = System.Array.IndexOf(aliens, alien);
        if (alienIndex != -1 && upAliens.Contains(alienIndex))
        {
            Vector3 initialPosition = alien.transform.position;
            Vector3 targetPosition = new Vector3(alien.transform.position.x, initialY, alien.transform.position.z);

            float popDownTime = Random.Range(minPopUpTime, maxPopUpTime);
            float timer = 0;
            while (timer < popDownTime)
            {
                alien.transform.position = Vector3.Lerp(initialPosition, targetPosition, timer / popDownTime);
                timer += Time.deltaTime;
                yield return null;
            }
            alien.transform.position = targetPosition;
            upAliens.Remove(alienIndex);
            Debug.Log($"{alien.name} moved down.");
        }
    }
}