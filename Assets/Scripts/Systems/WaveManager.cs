using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private float spawnBetweenWaves = 1.5f;
    private float spawnBetweenEnemies = .75f;

    [Header("Variable Wave Limits")]
    [SerializeField] private Vector3 minArea;
    [SerializeField] private Vector3 maxArea;

    [SerializeField] private ProgressBar progressBar;

    private WaveSO[] levelWaves;
    private int totalEnemies        = 0;
    private int totalKilledAmount   = 0;
    private int totalLeftAmount     = 0;

    public int EnemiesLeft
    {
        get => totalLeftAmount;
    }

    public int EnemiesKilled
    {
        get => totalKilledAmount;
    }

    public void IncrementKilled()
        => ++totalKilledAmount;
    public void IncrementLeft()
        => ++totalLeftAmount;

    public bool WaveComplete { get; private set; }

    public float EnemyProgress
    {
        get
        {
            int total = totalKilledAmount + totalLeftAmount;

            // Just indicate the level is done if no enemies are present
            if (total == totalEnemies)
            {
                WaveComplete = true;
                return 1f;
            }
            else if (totalEnemies is 0)
                return 0f;

            return total / (float)totalEnemies;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void InjectWaveData(WaveSO[] waveData)
        => levelWaves = waveData;

    public void BeginLevel()
    {
        totalEnemies        = 0;

        foreach (WaveSO wave in levelWaves)
        {
            foreach (WaveGUI wg in wave.waves)
            {
                for (int i = 0; i < wg.enemies.Length; ++i)
                {
                    totalEnemies += wg.numberOfEnemies[i];

                    if (wg.enemies[i].TryGetComponent(out Spawner spawner))
                        totalEnemies += spawner.SpawnsRemaining;
                }
            }
        }

        totalKilledAmount   = 0;
        totalLeftAmount     = 0;
        WaveComplete        = false;

        progressBar.Reset();

        StartCoroutine(SpawnWaves());
        progressBar.ToggleLevelActivity();
    }

    //private IEnumerator SpawnWaves()
    //{
    //    foreach (WaveSO wave in levelWaves)
    //    {
    //        for (int i = 0; i < wave.waves.Length; ++i)
    //        {
    //            WaveGUI currentWave = wave.waves[i];

    //            // This loop limits the j to the enemy prefabs, 
    //            for (int j = 0; j < currentWave.enemies.Length; ++j)
    //            {
    //                GameObject enemyPrefab;
    //                int numberOfEnemies;
    //                float spawnDelay;
    //                GameObject path;
    //                GameObject pathway;
    //                Transform[] pathPoints;

    //                try
    //                {
    //                    enemyPrefab     = currentWave.enemies[j];
    //                    numberOfEnemies = currentWave.numberOfEnemies[j];
    //                    spawnDelay      = currentWave.spawnDelay[j];
    //                    path            = currentWave.pathway[j];

    //                    // Default is 0, 0
    //                    Vector3 posOfSpawnOfPath = transform.position;

    //                    // If the path is detected to be only 2, aka a travel across screen, then randomize the instantiation location 
    //                    if (path.transform.childCount is 2)
    //                    {
    //                        Transform pathCheck = path.transform.GetChild(0);

    //                        if (pathCheck.position.x is 0)  // Vertical Movement
    //                            posOfSpawnOfPath = new Vector3(UnityEngine.Random.Range(minArea.x, maxArea.x), 0f, 0f);
    //                        else                            // Horizontal Movements
    //                            posOfSpawnOfPath = new Vector3(0f, UnityEngine.Random.Range(minArea.y, maxArea.y), 0f);
    //                    }

    //                    pathway = Instantiate(path, posOfSpawnOfPath, Quaternion.identity, transform);
    //                    pathPoints = pathway.GetComponent<Pathway>().GetWavePoints();
    //                }
    //                catch (IndexOutOfRangeException ex)
    //                {
    //                    Debug.Log($"Mismatch in wave {wave.name} in SpawnWaves()\nLengths:\nEnemyPrefabs: {currentWave.enemies.Length}" +
    //                        $"\nNumbers: {currentWave.numberOfEnemies.Length}\nSpawns: {currentWave.spawnDelay.Length}\nException:\n{ex}");
    //                    break;
    //                }

    //                for (int k = 0; k < numberOfEnemies; ++k)
    //                {
    //                    GameObject enemyInstance = Pools.Instance.SpawnObject(Pools.PoolType.Enemy, enemyPrefab, pathPoints[0].position, Quaternion.identity);
    //                    enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
    //                    yield return new WaitForSeconds(spawnDelay);
    //                }
    //            }

    //            yield return new WaitForSeconds(spawnBetweenEnemies);
    //        }

    //        yield return new WaitForSeconds(spawnBetweenWaves);
    //    }
    //}

    private IEnumerator SpawnWaves()
    {
        foreach (WaveSO wave in levelWaves)
        {
            bool allAtOnce      = wave.spawnAtOnce;
            float timeBetween   = wave.timeBetweenWaves;
            WaveGUI[] subWaves  = wave.waves;

            for (int i = 0; i < subWaves.Length; ++i)
            {
                WaveGUI currentGroup        = subWaves[i];
                bool spawnAllGroupsAtOnce   = currentGroup.spawnAllGroupsAtOnce;
                float timeBetweenGroups     = currentGroup.timeBetweenGroups;

                GameObject[] enemyPrefabs   = currentGroup.enemies;
                int[] numberOfEnemies       = currentGroup.numberOfEnemies;
                float[] spawnDelays         = currentGroup.spawnDelay;
                GameObject[] pathways       = currentGroup.pathway;

                // TODO ---> Come back to this to prevent this double up of code in multiple places

                // Every sub wave will spawn at once using a separate coroutine to juggle the spawning
                if (allAtOnce)
                    StartCoroutine(SpawnWaveAtOnce(enemyPrefabs, numberOfEnemies, spawnDelays, pathways, spawnAllGroupsAtOnce, timeBetweenGroups));
                else
                {
                    for (int j = 0; j < enemyPrefabs.Length; ++j)
                    {
                        GameObject currentPrefab = enemyPrefabs[j];
                        int currentAmount = numberOfEnemies[j];
                        float currentDelay = spawnDelays[j];
                        GameObject currentPathway = pathways[j];

                        // Default is 0, 0
                        Vector3 posOfSpawnOfPath = transform.position;

                        // If the path is detected to be only 2, aka a travel across screen, then randomize the instantiation location 
                        if (currentPathway.transform.childCount is 2)
                        {
                            Transform pathCheck = currentPathway.transform.GetChild(0);

                            if (pathCheck.position.x is 0)  // Vertical Movement
                                posOfSpawnOfPath = new Vector3(UnityEngine.Random.Range(minArea.x, maxArea.x), 0f, 0f);
                            else                            // Horizontal Movements
                                posOfSpawnOfPath = new Vector3(0f, UnityEngine.Random.Range(minArea.y, maxArea.y), 0f);
                        }

                        GameObject pathway = Instantiate(currentPathway, posOfSpawnOfPath, Quaternion.identity, transform);
                        Transform[] pathPoints = pathway.GetComponent<Pathway>().GetWavePoints();

                        // Another Coroutine is used so this coroutine continues to run and starts the coroutine groups to spawn along this one
                        if (spawnAllGroupsAtOnce)
                            StartCoroutine(SpawnGroupAtOnce(currentPrefab, currentAmount, currentDelay, pathPoints));
                        // Otherwise, this coroutine will handle the spawning group by group
                        else
                        {
                            for (int k = 0; k < currentAmount; ++k)
                            {
                                GameObject enemyInstance = Pools.Instance.SpawnObject(Pools.PoolType.Enemy, currentPrefab, pathPoints[0].position, Quaternion.identity);
                                enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
                                yield return new WaitForSeconds(currentDelay);
                            }

                            yield return new WaitForSeconds(timeBetweenGroups);
                        }
                    }

                    yield return new WaitForSeconds(timeBetween);
                }
            }
        }
    }

    private IEnumerator SpawnGroupAtOnce(GameObject currentPrefab, int currentAmount, float currentDelay, Transform[] pathPoints)
    {
        for (int k = 0; k < currentAmount; ++k)
        {
            GameObject enemyInstance = Pools.Instance.SpawnObject(Pools.PoolType.Enemy, currentPrefab, pathPoints[0].position, Quaternion.identity);
            enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
            yield return new WaitForSeconds(currentDelay);
        }
    }

    private IEnumerator SpawnWaveAtOnce(GameObject[] enemyPrefabs, int[] numberOfEnemies, float[] spawnDelays, GameObject[] pathways, bool spawnAllGroupsAtOnce, float timeBetweenGroups)
    {
        for (int j = 0; j < enemyPrefabs.Length; ++j)
        {
            GameObject currentPrefab = enemyPrefabs[j];
            int currentAmount = numberOfEnemies[j];
            float currentDelay = spawnDelays[j];
            GameObject currentPathway = pathways[j];

            // Default is 0, 0
            Vector3 posOfSpawnOfPath = transform.position;

            // If the path is detected to be only 2, aka a travel across screen, then randomize the instantiation location 
            if (currentPathway.transform.childCount is 2)
            {
                Transform pathCheck = currentPathway.transform.GetChild(0);

                if (pathCheck.position.x is 0)  // Vertical Movement
                    posOfSpawnOfPath = new Vector3(UnityEngine.Random.Range(minArea.x, maxArea.x), 0f, 0f);
                else                            // Horizontal Movements
                    posOfSpawnOfPath = new Vector3(0f, UnityEngine.Random.Range(minArea.y, maxArea.y), 0f);
            }

            GameObject pathway = Instantiate(currentPathway, posOfSpawnOfPath, Quaternion.identity, transform);
            Transform[] pathPoints = pathway.GetComponent<Pathway>().GetWavePoints();

            // Another Coroutine is used so this coroutine continues to run and starts the coroutine groups to spawn along this one
            if (spawnAllGroupsAtOnce)
                StartCoroutine(SpawnGroupAtOnce(currentPrefab, currentAmount, currentDelay, pathPoints));
            // Otherwise, this coroutine will handle the spawning group by group
            else
            {
                for (int k = 0; k < currentAmount; ++k)
                {
                    GameObject enemyInstance = Pools.Instance.SpawnObject(Pools.PoolType.Enemy, currentPrefab, pathPoints[0].position, Quaternion.identity);
                    enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
                    yield return new WaitForSeconds(currentDelay);
                }

                yield return new WaitForSeconds(timeBetweenGroups);
            }
        }
    }
}
