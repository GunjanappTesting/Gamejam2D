using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public List<GameObject> prefabs; // List of prefabs to choose from
    public float spawnInterval = 2.0f; // Time interval between spawns
    public float distance;

    private float lastSpawnXPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        lastSpawnXPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        StartCoroutine(SpawnPrefabs());
    }

    IEnumerator SpawnPrefabs()
    {
        while (true)
        {

            // Choose a random prefab from the list
            GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Count)];

            // Calculate the spawn position
            Vector3 spawnPosition = new Vector3(lastSpawnXPosition, 0, 0);

            // Instantiate the prefab
            MoveLeft spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity).GetComponent<MoveLeft>();

            // Update the last spawn position
            lastSpawnXPosition += distance;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
// Separate class to handle manual movement if Rigidbody2D is not attached

