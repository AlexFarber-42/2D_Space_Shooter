using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

        levelsWon = 0;
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

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
