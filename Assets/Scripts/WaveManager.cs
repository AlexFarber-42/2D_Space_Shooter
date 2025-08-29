using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveSO[] levelWaves;
    [SerializeField] private Transform enemyPool;
    private float spawnBetweenEnemies = .75f;

    [Header("Variable Wave Limits")]
    [SerializeField] private Vector3 minArea;
    [SerializeField] private Vector3 maxArea;

    // This map will keep track of each pathway's number of enemies for deletion
    private Dictionary<GameObject, Transform> enemyMap = new Dictionary<GameObject, Transform>();

    public void BeginLevel()
    {
        StartCoroutine(SpawnWaves());
    }

    private void FixedUpdate()
    {
        if (enemyMap.Count is 0)
            return;

        // Check if a pathway is cleared to delete it and its entry in the dictionary
        Queue<GameObject> gc = new Queue<GameObject>();

        foreach (KeyValuePair<GameObject, Transform> list in enemyMap)
        {
            if (list.Value.childCount is 0)
                gc.Enqueue(list.Key);
        }

        int gcCount = gc.Count;
        for (int i = 0; i < gcCount; ++i)
        {
            GameObject go = gc.Dequeue();
            Destroy(enemyMap[go].gameObject);
            enemyMap.Remove(go);
            Destroy(go);
        }
    }

    private IEnumerator SpawnWaves()
    {
        foreach (WaveSO wave in levelWaves)
        {
            GameObject path = wave.pathway;

            // Default is 0, 0
            Vector3 posOfSpawnOfPath = transform.position;

            // If the path is detected to be only 2, aka a travel across screen, then randomize the instantiation location 
            if (path.transform.childCount is 2)
            {
                Transform pathCheck = path.transform.GetChild(0);

                if (pathCheck.position.x is 0)  // Vertical Movement
                    posOfSpawnOfPath = new Vector3(UnityEngine.Random.Range(minArea.x, maxArea.x), 0f, 0f);
                else                            // Horizontal Movements
                    posOfSpawnOfPath = new Vector3(0f, UnityEngine.Random.Range(minArea.y, maxArea.y), 0f);
            }

            GameObject pathway = Instantiate(path, posOfSpawnOfPath, Quaternion.identity, transform);
            Transform[] pathPoints = pathway.GetComponent<Pathway>().GetWavePoints();

            for (int i = 0; i < wave.waves.Length; ++i)
            {
                WaveGUI currentWave = wave.waves[i];
                GameObject pathPool = new GameObject($"Path Pool {i + 1}");
                pathPool.transform.SetParent(enemyPool.transform);

                // This loop limits the j to the enemy prefabs, 
                for (int j = 0; j < currentWave.enemies.Length; ++j)
                {
                    GameObject enemyPrefab;
                    int numberOfEnemies;
                    float spawnDelay;

                    try
                    {
                        enemyPrefab     = currentWave.enemies[j];
                        numberOfEnemies = currentWave.numberOfEnemies[j];
                        spawnDelay      = currentWave.spawnDelay[j];
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        Debug.Log($"Mismatch in wave {wave.name} in SpawnWaves()\nLengths:\nEnemyPrefabs: {currentWave.enemies.Length}" +
                            $"\nNumbers: {currentWave.numberOfEnemies.Length}\nSpawns: {currentWave.spawnDelay.Length}\nException:\n{ex}");
                        break;
                    }

                    for (int k = 0; k < numberOfEnemies; ++k)
                    {
                        GameObject enemyInstance = Instantiate(enemyPrefab, pathPoints[0].position, Quaternion.identity, pathPool.transform);
                        enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
                        yield return new WaitForSeconds(spawnDelay);
                    }
                }

                enemyMap.Add(pathway, pathPool.transform);

                yield return new WaitForSeconds(spawnBetweenEnemies);
            }

            yield return new WaitForSeconds(spawnBetweenEnemies);
        }
    }
}
