using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random; // Alias Unity's Random to avoid conflict with System.Random

public class ObstacleGenerator : MonoBehaviour
{
    public List<GameObject> prefabs; // List of prefabs to choose from
    public float spawnInterval = 2.0f; // Time interval between spawns
    public float distance;
    public Transform cameraHold;

    private float lastSpawnXPosition;
    private Camera mainCamera;

    public List<MoveLeft> allSpawnPrefab;

    private void OnEnable()
    {
        GameManager.PlayerHitFired += GameEndFunction;
    }

    private void OnDisable()
    {
        GameManager.PlayerHitFired -= GameEndFunction;
    }

    void Start()
    {
        Initialize();
        StartCoroutine(SpawnPrefabs());
    }

    private void Initialize()
    {
        mainCamera = Camera.main;
        lastSpawnXPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
    }

    IEnumerator SpawnPrefabs()
    {
        yield return new WaitForSeconds(1f);

        while (GameManager.isAlive)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnPrefab()
    {
        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Count)];
        Vector3 spawnPosition = new Vector3(lastSpawnXPosition, 0, 0);
        MoveLeft spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity).GetComponent<MoveLeft>();
        allSpawnPrefab.Add(spawnedPrefab);
        lastSpawnXPosition += distance;
    }

    public void GameEndFunction()
    {
        GameManager.isAlive = false;
        StartCoroutine(ReduceTimeScale());
        /* Time.timeScale = 0;
         GameManager.GameEnd?.Invoke();*/
        /*StartCoroutine(ReduceTimeScale());
        cameraHold.DOShakePosition(1f, 1f, 1);*/
        /*.OnComplete(() =>
            cameraHold.DOMoveX(5f, 5f).SetEase(Ease.InElastic)
        );*/

    }
    private IEnumerator ReduceTimeScale()
    {
        float start = Time.timeScale;
        float end = 0f;
        float elapsed = 0f;

        while (elapsed < 1)
        {
            Time.timeScale = Mathf.Lerp(start, end, elapsed / 2);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        GameManager.GameEnd?.Invoke();
    }
}
