using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnInterval = 2f;  // Time between spawns
    public float minX = -10f;
    public float maxX = 10f;
    public float startY = -5f;  // Starting Y position
    public float startZ = -5f;
    public int maxObjects = 10;  // Maximum number of objects that can be active at the same time
    public float lineOffset = 2f;  // Distance between two lines of objects
    public Material specialMaterial;         // New Material for special objects
    public int bonusPoints = 20;             // Extra points for special objects
    public float specialChance = 0.1f; 

    private List<GameObject> spawnedObjects = new List<GameObject>();  // List to track active objects

    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (true)
        {
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
            
            var pokeable = obj.GetComponent<Mover>();
            if (pokeable != null)
            {
                pokeable.extraPoints = bonusPoints;
            }
        }
    }

    private void CleanupObjects()
    {
        spawnedObjects.RemoveAll(item => item == null);  // Clean up list to remove destroyed objects
    }
}
