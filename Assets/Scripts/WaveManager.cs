using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveSO[] levelWaves;
    [SerializeField] private Transform enemyPool;
    private float spawnBetweenEnemies = .75f;

    public void BeginLevel()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        foreach (WaveSO wave in levelWaves)
        {
            for (int i = 0; i < wave.waves.Length; ++i)
            {
                WaveGUI currentWave = wave.waves[i];

                for (int j = 0; j < currentWave.enemies.Length; ++j)
                {
                    GameObject enemyPrefab  = currentWave.enemies[j];
                    int numberOfEnemies     = currentWave.numberOfEnemies[j];
                    float spawnDelay        = currentWave.spawnDelay[j];

                    GameObject pathway = Instantiate(wave.pathway, transform.position, Quaternion.identity, transform);
                    Transform[] pathPoints = pathway.GetComponent<Pathway>().GetWavePoints();

                    for (int k = 0; k < numberOfEnemies; ++k)
                    {
                        GameObject enemyInstance = Instantiate(enemyPrefab, pathPoints[0].position, Quaternion.identity, enemyPool);
                        enemyInstance.GetComponent<Enemy>().SetPath(pathPoints[1..pathPoints.Length]);
                        yield return new WaitForSeconds(spawnDelay / 2f);
                    }

                    yield return new WaitForSeconds(spawnDelay);
                }

                yield return new WaitForSeconds(spawnBetweenEnemies);
            }

            yield return new WaitForSeconds(spawnBetweenEnemies);
        }
    }
}
