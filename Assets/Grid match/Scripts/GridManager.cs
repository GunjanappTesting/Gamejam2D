using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public int gridSize = 8;
    public GameObject[] objectPrefabs; // Prefabs for different object types
    public GameObject[,] grid;
    private float speedMultiplayer = .1f;
    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;
        //Time.timeScale = .05f;
    }

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new GameObject[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int randomIndex = Random.Range(0, objectPrefabs.Length);
                GameObject obj = Instantiate(objectPrefabs[randomIndex], new Vector2(x, y), Quaternion.identity);
                obj.transform.parent = this.transform;
                obj.name = "(" + x + "," + y + ")";
                grid[x, y] = obj;
            }
        }
    }

    public void OnObjectClicked(GameObject obj)
    {
        Vector2Int gridPos = FindObjectInGrid(obj);
        if (gridPos != -Vector2Int.one)
        {
            List<GameObject> matchingObjects = FindMatchingObjects(gridPos.x, gridPos.y, obj.tag);
            if (matchingObjects.Count > 1)
            {
                DestroyObjects(matchingObjects);
            }
            else
            {
                Destroy(obj);
                Invoke(nameof(ApplyGravity), .01f);
            }

            /// single mood for test
            // Destroy(obj);
            // Invoke(nameof(ApplyGravity), .01f);
        }
    }

    Vector2Int FindObjectInGrid(GameObject obj)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == obj)
                    return new Vector2Int(x, y);
            }
        }
        return -Vector2Int.one; // Not found
    }

    List<GameObject> FindMatchingObjects(int x, int y, string tag)
    {
        List<GameObject> matchingObjects = new List<GameObject>();
        bool[,] visited = new bool[gridSize, gridSize];
        SearchMatchingObjects(x, y, tag, visited, matchingObjects);
        return matchingObjects;
    }

    void SearchMatchingObjects(int x, int y, string tag, bool[,] visited, List<GameObject> matchingObjects)
    {

        //if (x < 0 || y < 0 || x >= gridSize || y >= gridSize || grid[x, y].gameObject == null || visited[x, y] || grid[x, y].tag != tag)
        //    return;
        // Check if x is out of bounds (less than 0)
        if (x < 0)
            return;

        // Check if y is out of bounds (less than 0)
        if (y < 0)
            return;

        // Check if x exceeds the grid size
        if (x >= gridSize)
            return;

        // Check if y exceeds the grid size
        if (y >= gridSize)
            return;

        // Check if there is no object in the grid cell
        if (grid[x, y].gameObject == null)
            return;

        // Check if the cell has already been visited
        if (visited[x, y])
            return;

        // Check if the tag does not match the required tag
        if (grid[x, y].tag != tag)
            return;

        visited[x, y] = true;
        matchingObjects.Add(grid[x, y]);

        // Check adjacent cells
        SearchMatchingObjects(x + 1, y, tag, visited, matchingObjects);
        SearchMatchingObjects(x - 1, y, tag, visited, matchingObjects);
        SearchMatchingObjects(x, y + 1, tag, visited, matchingObjects);
        SearchMatchingObjects(x, y - 1, tag, visited, matchingObjects);
    }

    void DestroyObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
            //Invoke(nameof(ApplyGravity), .01f);
            // Optionally, add particle effects or score increment here
        }
        Invoke(nameof(ApplyGravity), .01f);
    }
    void ApplyGravity()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 1; y < gridSize; y++) // Start from the second row (y=1) because bottom row can't fall further
            {
                if (grid[x, y] != null && grid[x, y - 1] == null)
                {
                    int fallTo = y - 1;
                    // Find the lowest empty cell directly below
                    while (fallTo > 0 && grid[x, fallTo - 1] == null)
                    {
                        fallTo--;
                    }

                    // Move the object down
                    grid[x, fallTo] = grid[x, y];
                    grid[x, y] = null;
                    float time = Mathf.Abs(fallTo - grid[x, fallTo].transform.position.y) * speedMultiplayer;
                    // Update object position in the scene
                    grid[x, fallTo].transform.DOKill();
                    grid[x, fallTo].transform.DOMove(new Vector2(x, fallTo), time).SetEase(Ease.Linear);
                }
            }
        }

        // Fill top empty cells with new objects
        FillTopRow();
    }

    void FillTopRow()
    {
        //for (int x = 0; x < gridSize; x++)
        //{
        //    if (grid[x, gridSize - 1] == null) // Check top row (y = gridSize - 1)
        //    {
        //        int randomIndex = Random.Range(0, objectPrefabs.Length);
        //        GameObject newObj = Instantiate(objectPrefabs[randomIndex], new Vector2(x, gridSize), Quaternion.identity);
        //        newObj.transform.DOKill();
        //        newObj.transform.DOMoveY(newObj.transform.position.y - 1, speedMultiplayer).SetEase(Ease.Linear);
        //        grid[x, gridSize - 1] = newObj;
        //        newObj.transform.parent = this.transform;
        //        newObj.name = "(" + x + "," + (gridSize - 1) + ")";
        //    }
        //}

        for (int x = 0; x < gridSize; x++)
        {
            int count = 0;
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == null)
                {
                    int randomIndex = Random.Range(0, objectPrefabs.Length);
                    GameObject newObj = Instantiate(objectPrefabs[randomIndex], new Vector2(x, gridSize + count), Quaternion.identity);
                    count++;
                    float time = Mathf.Abs(newObj.transform.position.y - y) * speedMultiplayer;
                    newObj.transform.DOKill();
                    newObj.transform.DOMoveY(y, time).SetEase(Ease.Linear);
                    grid[x, y] = newObj;
                    newObj.transform.parent = this.transform;
                    newObj.name = "(" + x + "," + (gridSize - 1) + ")";
                }
            }
        }
    }
}
