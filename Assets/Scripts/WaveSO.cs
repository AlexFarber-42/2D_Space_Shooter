using UnityEngine;

[CreateAssetMenu(fileName = "WaveSO", menuName = "Scriptable Objects/WaveSO")]
public class WaveSO : ScriptableObject
{
    public WaveGUI[] waves;
    public GameObject pathway;
}

[System.Serializable]
public class WaveGUI
{
    [HideInInspector] public string waveName = "Wave";

    public GameObject[] enemies;
    public int[] numberOfEnemies;
    public float[] spawnDelay;
}
