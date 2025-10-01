using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles unique spawning for other entities from this object
/// </summary>
public class Spawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The entity that will spawn from this object")]
    private GameObject enemyPrefab;

    [SerializeField]
    [Tooltip("Total number of enemies that will spawn from this object")]
    private int totalNumberToSpawn;

    [SerializeField]
    [Tooltip("The time between each spawn")]
    private float spawnDelay = 1.0f;

    [Tooltip("Flag for if this object should burst after the totalNumberToSpawn is fully passed for extras at the end")]
    public bool burstAtEnd;

    [SerializeField]
    private int numberAtBurst;

    [SerializeField] private bool targetPlayer;
    [SerializeField]
    [Tooltip("Use the V2Ints as normalized directions, i.e. (0,1) is up and (-1, 1) moves toward the top left")]
    private List<Vector2Int> paths;

    // Property to help with score calculation
    public int SpawnsRemaining
    {
        get => totalNumberToSpawn - spawnCount;
    }

    public int EnemyScoreCheck
    {
        get => enemyPrefab.GetComponent<Enemy>().GetScoreValue();
    }

    private Transform playerTransRetent;

    private void Awake()
    {
        if (targetPlayer)
            playerTransRetent = Player.Instance.transform;
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private int spawnCount;

    private IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnUnit();

            if (spawnCount == totalNumberToSpawn)
            {
                if (burstAtEnd)
                {
                    for (int i = 0; i < numberAtBurst; ++i)
                    {
                        SpawnUnit();

                        yield return new WaitForSeconds(.1f);
                    }
                }

                // Exit out of the loop
                break;
            }
        }

        // TODO ---> Add a breakup effect or explosion effect at end with a delay

        Destroy(gameObject);

        void SpawnUnit()
        {
            GameObject enemy = Pools.Instance.SpawnObject(Pools.PoolType.Enemy, enemyPrefab, transform.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().SetPath(new Transform[1] { GeneratePath() });
            ++spawnCount;
        }
    }

    private readonly int spotPlayerThreshold = 55;

    private Transform GeneratePath()
    {
        // First randomly calculate if this path will move towards the player
        if (targetPlayer)
        {
            bool willTargetPlayer = UnityEngine.Random.Range(0, 101) > spotPlayerThreshold;

            // Give path to player
            if (willTargetPlayer)
                return playerTransRetent;
        }

        try
        {
            Vector2Int chosenPath = paths[UnityEngine.Random.Range(0, paths.Count)];
            return BoundaryHandler.Instance.RetrieveTransform(chosenPath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Exception detected when attempting to generate a path for a spawned enemy\nException: {ex}");
            return transform; // Simply return this transform so the enemy instantly destroys itself upon reaching its own spawn destination
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private Spawner container;

    private void OnEnable()
    {
        container = target as Spawner;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Spawner Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("totalNumberToSpawn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnDelay"));
        EditorGUILayout.Space(1);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("burstAtEnd"));

        if (container.burstAtEnd)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberAtBurst"));
        }

        EditorGUILayout.Space(5);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Path Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPlayer"));
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("paths"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif