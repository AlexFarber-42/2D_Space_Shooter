using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Tooltip("Represents the order in relation to the other levels")] public int levelIndex; 
    public string LevelName;
    public Sprite LevelSprite;

    public GameObject BackgroundData; // The background object to be spawned in upon level loading
    public WaveSO[] Waves;            // The waves that make up this level
    // TODO ---> To be further filled out <---
}
