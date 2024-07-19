using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; } // Singleton instance
    public GameObject objectToSpawn;
    public float spawnInterval = 2f;  // Time between spawns
    public float minX = -10f;
    public float maxX = 10f;
    public float startY = -5f;  // Starting Y position
    public float startZ = -5f;
    public int maxObjects = 10;  // Maximum number of objects that can be active at the same time
    public float lineOffset = 2f;  // Distance between two lines of objects
    public Material specialMaterial;  // New Material for special objects
    public int bonusPoints = 20;  // Extra points for special objects
    public float specialChance = 0.1f;
    public float specialSpeedMultiplier = 1.2f;  // Speed multiplier for special objects
    public float specialHorizontalMagnitudeMultiplier = 1.5f;  // Horizontal magnitude multiplier for special objects

    private List<GameObject> spawnedObjects = new List<GameObject>();  // List to track active objects
    private ControllerManager controllerManager;
    private LevelManager levelManager;
    private bool isSpawningStopped = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        controllerManager = FindObjectOfType<ControllerManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null || levelManager.levels.Count == 0)
        {
            Debug.LogError("LevelManager not found or no levels defined.");
            return;
        }
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (!isSpawningStopped)
        {
            if (controllerManager != null && controllerManager.IsPaused())
            {
                yield return null;
                continue;
            }

            if (spawnedObjects.Count < maxObjects - 1)
            {
                float baseX = Random.Range(minX, maxX);
                Quaternion spawnRotation = Quaternion.Euler(0, 270, 0);
                // Randomly decide if this will be a special object
                bool isSpecial = Random.value < specialChance;

                // Spawn first object
                GameObject firstObject = Instantiate(objectToSpawn, new Vector3(baseX, startY, startZ), spawnRotation);
                SetupObject(firstObject, isSpecial);

                // Spawn second object slightly offset from the first
                GameObject secondObject = Instantiate(objectToSpawn, new Vector3(baseX, startY - 0.5f, startZ + lineOffset), spawnRotation);
                SetupObject(secondObject, isSpecial);

                spawnedObjects.Add(firstObject);
                spawnedObjects.Add(secondObject);
            }
            CleanupObjects();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SetupObject(GameObject obj, bool isSpecial)
    {
        var mover = obj.GetComponent<Mover>();
        Level currentLevel = levelManager.levels[levelManager.getCurrentLevel()];
        if (isSpecial)
        {
            Renderer plateVisualRenderer = obj.transform.Find("Visuals/PlateVisual/rozatutorialv2").GetComponent<Renderer>();

            if (plateVisualRenderer != null)
            {
                plateVisualRenderer.material = specialMaterial;
            }
            else
            {
                Debug.LogError("Renderer not found in the instantiated object!");
            }

            if (mover != null)
            {
                mover.extraPoints = bonusPoints;
                mover.SetProperties(currentLevel.verticalSpeed * specialSpeedMultiplier, currentLevel.maxVerticalSpeed * specialSpeedMultiplier, currentLevel.horizontalMagnitude * specialHorizontalMagnitudeMultiplier);
                mover.SetSpecial(true);
            }
        }
        else if (mover != null)
        {
            mover.SetProperties(currentLevel.verticalSpeed, currentLevel.maxVerticalSpeed, currentLevel.horizontalMagnitude);
            mover.SetSpecial(false);
        }
    }

    public void DisableAllObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    public void EnableAllObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void CleanupObjects()
    {
        spawnedObjects.RemoveAll(item => item == null);  // Clean up list to remove destroyed objects
    }

    public void StopSpawning()
    {
        isSpawningStopped = true;
    }
}
