using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly string LevelsDataPath = "ScriptableObjects/Levels";
    private LevelSO[] levels;
    private Dictionary<LevelSO, bool> LevelsComplete;

    public LevelSO CurrentLevel
    {
        get => currentLevel;
    }

    private int currentIndex;
    private LevelSO currentLevel;
    private GameObject spawnedBackground;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // TODO ---> Add an if statement for saving functionality 

        levelsWon = 0;
        currentIndex = 0;
        
        LevelsComplete = new Dictionary<LevelSO, bool>();
        levels = Resources.LoadAll<LevelSO>(LevelsDataPath); 

        for (int i = 0; i < levels.Length; ++i)
            LevelsComplete.Add(levels[i], false);
    }

    public bool WonLevel
    {
        get; private set;
    }

    private int levelsWon = 0;

    public void SignalLevelComplete()
    {
        WonLevel = true;
        levelsWon++;
        currentIndex++;
        LevelsComplete[currentLevel] = true;

        LevelCompletionStats levelStats = FindFirstObjectByType<LevelCompletionStats>();
        levelStats.SignalLevelNotifActive(true);
        levelStats.InjectStats();
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    private void ActiveSceneChanged(Scene current, Scene next)
    {
        switch (next.buildIndex)
        {
            case 0: // Title Screen
                break;
            case 1: // Game Screen
                FindFirstObjectByType<LevelCompletionStats>().SignalLevelNotifActive(false);
                currentLevel = levels[currentIndex];
                spawnedBackground = Instantiate(currentLevel.BackgroundData);
                WaveManager.Instance.InjectWaveData(currentLevel.Waves);
                break;
            case 2: // Shop Screen
                if (WonLevel)
                {
                    for (int i = 0; i < levelsWon; ++i) 
                        UpgradeGenerator.Instance.LoadAnUpgrade();

                    WonLevel = false;
                    levelsWon = 0;
                }
                break;
        }
    }

    public void DestroyBackground()
    {
        if (spawnedBackground != null)
            Destroy(spawnedBackground);
    }

    public void ChangeScene(int sceneIndex)
    {
        DestroyBackground();
        SceneManager.LoadScene(sceneIndex);
    }
}
