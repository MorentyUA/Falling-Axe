using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CubeSpawnData
{
    public GameObject cubePrefab;
    [Range(0f, 100f)]
    public float spawnChance = 10f; // Шанс в процентах
}

public class RowSpawner : MonoBehaviour
{
    [Header("Cube Prefabs with Spawn Chances")]
    public CubeSpawnData[] cubeTypes; // Массив кубов с шансами

    [Header("Spawn Settings")]
    public float cubeSize = 1f;
    public float horizontalSpacing = 0f;
    public float verticalSpacing = 0f;
    public float spawnDistance = 15f;

    private Camera mainCamera;
    private float lastSpawnY;
    private float screenHalfWidth;
    private List<GameObject> spawnedCubes = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        screenHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        lastSpawnY = mainCamera.transform.position.y - spawnDistance;

        // Генерируем начальные ряды
        for (int i = 0; i < 25; i++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        if (mainCamera.transform.position.y - spawnDistance < lastSpawnY)
        {
            SpawnRow();
        }

        CleanupCubes();
    }

    void SpawnRow()
    {
        if (cubeTypes.Length == 0) return;

        float availableWidth = screenHalfWidth * 2;
        float cubeWithSpacing = cubeSize + horizontalSpacing;
        int cubesInRow = Mathf.FloorToInt(availableWidth / cubeWithSpacing);

        float totalRowWidth = (cubesInRow * cubeSize) + ((cubesInRow - 1) * horizontalSpacing);
        float startX = -totalRowWidth / 2f + cubeSize / 2f;

        for (int i = 0; i < cubesInRow; i++)
        {
            float posX = startX + i * cubeWithSpacing;
            Vector3 spawnPosition = new Vector3(posX, lastSpawnY, 0);

            // Выбираем куб по шансу
            GameObject cubePrefab = SelectCubeByChance();

            if (cubePrefab != null)
            {
                GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
                spawnedCubes.Add(cube);
            }
        }

        lastSpawnY -= (cubeSize + verticalSpacing);
    }

    GameObject SelectCubeByChance()
    {
        // Считаем общую сумму шансов
        float totalChance = 0f;
        foreach (var cubeData in cubeTypes)
        {
            totalChance += cubeData.spawnChance;
        }

        if (totalChance <= 0) return null;

        // Генерируем случайное число
        float randomValue = Random.Range(0f, totalChance);

        // Выбираем куб на основе шанса
        float currentChance = 0f;
        foreach (var cubeData in cubeTypes)
        {
            currentChance += cubeData.spawnChance;
            if (randomValue <= currentChance)
            {
                return cubeData.cubePrefab;
            }
        }

        // На всякий случай возвращаем первый
        return cubeTypes[0].cubePrefab;
    }

    void CleanupCubes()
    {
        float cleanupHeight = mainCamera.transform.position.y + spawnDistance + 5f;

        for (int i = spawnedCubes.Count - 1; i >= 0; i--)
        {
            if (spawnedCubes[i] == null)
            {
                spawnedCubes.RemoveAt(i);
                continue;
            }

            if (spawnedCubes[i].transform.position.y > cleanupHeight)
            {
                Destroy(spawnedCubes[i]);
                spawnedCubes.RemoveAt(i);
            }
        }
    }
}
