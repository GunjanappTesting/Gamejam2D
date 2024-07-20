using UnityEngine;
using System.Collections.Generic;

public class OpticalSpawner : MonoBehaviour
{
    public GameObject pillarPrefab;
    public Transform upperPosition;
    public Transform lowerPosition;
    public Transform destroyPoint;
    public float moveSpeed = 5f;
    public float spawnInterval = 2f;
    public float minHeight = 1f;
    public float maxHeight = 5f;

    private float timeSinceLastSpawn;
    private List<GameObject> pillars = new List<GameObject>();

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnPillars();
            timeSinceLastSpawn = 0f;
        }

        MoveAndDestroyPillars();
    }

    void SpawnPillars()
    {
        float lowerHeight = Random.Range(minHeight, maxHeight);
        float upperHeight = GetUpperHeight(lowerHeight);

        GameObject lowerPillar = Instantiate(pillarPrefab, lowerPosition.position, Quaternion.identity);
        SetPillarHeight(lowerPillar, lowerHeight);

        GameObject upperPillar = Instantiate(pillarPrefab, upperPosition.position, Quaternion.Euler(0, 0, 180));
        SetPillarHeight(upperPillar, upperHeight);

        pillars.Add(lowerPillar);
        pillars.Add(upperPillar);
    }

    float GetUpperHeight(float lowerHeight)
    {
        // Define relationships between lowerHeight and upperHeight
        if (lowerHeight <= 1)
            return 4f;
        if (lowerHeight <= 2)
            return 3f;
        if (lowerHeight <= 4)
            return 2f;
        if (lowerHeight <= 5)
            return 1f;

        // Default fallback if no condition matches
        return minHeight;
    }

    void SetPillarHeight(GameObject pillar, float height)
    {
        float width = Random.Range(minHeight, maxHeight);
        Vector3 scale = pillar.transform.localScale;
        scale.y = height;
        scale.x = width;
        pillar.transform.localScale = scale;
    }

    void MoveAndDestroyPillars()
    {
        for (int i = pillars.Count - 1; i >= 0; i--)
        {
            GameObject pillar = pillars[i];
            pillar.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (pillar.transform.position.x < destroyPoint.position.x)
            {
                Destroy(pillar);
                pillars.RemoveAt(i);
            }
        }
    }
}
