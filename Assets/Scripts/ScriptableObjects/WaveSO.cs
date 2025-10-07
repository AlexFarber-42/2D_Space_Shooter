using UnityEngine;

[CreateAssetMenu(fileName = "WaveSO", menuName = "Scriptable Objects/WaveSO")]
public class WaveSO : ScriptableObject
{
    public WaveGUI[] waves;
    public bool spawnAtOnce;
    public float timeBetweenWaves;
}

[System.Serializable]
public class WaveGUI
{
    [HideInInspector] public string waveName = "Wave";

    public bool spawnAllGroupsAtOnce;
    public float timeBetweenGroups;
    public GameObject[] enemies;
    public int[] numberOfEnemies;
    public float[] spawnDelay;
    public GameObject[] pathway;

    public WaveGUI()
    {
        enemies              = new GameObject[0];
        numberOfEnemies      = new int[0];
        spawnDelay           = new float[0];
        pathway              = new GameObject[0];
        timeBetweenGroups    = .5f;
        spawnAllGroupsAtOnce = false;
    }

    public WaveGUI(int size)
    {
        enemies         = new GameObject[size];
        numberOfEnemies = new int[size];
        spawnDelay      = new float[size];
        pathway         = new GameObject[size];

        timeBetweenGroups = .5f;
        spawnAllGroupsAtOnce = false;
    }
}
