using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<Level> levels;
    private int currentLevelIndex = 0;
    private int objectsHitInCurrentLevel = 0;

    private Spawner spawner;
    private bool isGameFinished = false;

    private void Start()
    {
        spawner = FindObjectOfType<Spawner>();
        SetLevel(currentLevelIndex);
    }

    public void SetLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            EndGame();
            return;
        }

        currentLevelIndex = levelIndex;
        Level currentLevel = levels[currentLevelIndex];
        spawner.spawnInterval = currentLevel.spawnInterval;
        spawner.maxObjects = currentLevel.maxObjects;
        spawner.specialChance = currentLevel.specialChance;

        // Adjust properties for Mover class globally
        Mover[] movers = FindObjectsOfType<Mover>();
        foreach (var mover in movers)
        {
            mover.SetProperties(currentLevel.verticalSpeed, currentLevel.maxVerticalSpeed, currentLevel.horizontalMagnitude);
        }

        // Destroy all currently spawned objects
        DestroyAllActiveObjects();

        Debug.Log("Level " + (currentLevelIndex + 1) + " started.");
    }

    public void OnObjectHit()
    {
        if (isGameFinished) return;

        objectsHitInCurrentLevel++;

        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Count)
        {
            Debug.LogWarning("Attempted to access an invalid level index: " + currentLevelIndex);
            return;
        }

        if (objectsHitInCurrentLevel >= levels[currentLevelIndex].objectsToHitForNextLevel)
        {
            objectsHitInCurrentLevel = 0;
            Debug.Log("Level " + (currentLevelIndex + 1) + " completed.");
            currentLevelIndex++;
            SetLevel(currentLevelIndex);
        }
    }

    private void EndGame()
    {
        isGameFinished = true;
        spawner.StopSpawning();
        DestroyAllActiveObjects();
        Debug.Log("Game Finished!");
        ScoreManager.Instance.DisplayGameFinished();
    }

    private void DestroyAllActiveObjects()
    {
        Mover[] movers = FindObjectsOfType<Mover>();
        foreach (var mover in movers)
        {
            Destroy(mover.gameObject);
        }
    }

    public void OnGameOver()
    {
        // Handle game over logic
        Debug.Log("Game Over");
    }

    public int getCurrentLevel()
    {
        return currentLevelIndex;
    }
}
