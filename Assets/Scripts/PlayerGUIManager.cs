using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUIManager : MonoBehaviour
{
    public static PlayerGUIManager Instance { get; private set; }

    [Header("GUI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform progressBar;
    [SerializeField] private Image powerUpImage;

    private Player playerRef;
    private GameObject healthPoint;
    private int scoreTracker;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        healthPoint = Resources.Load<GameObject>("Prefabs/UI/Health_Point");
        playerRef = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        UpdateScore();
        powerUpImage.enabled = false;

        ResetHealthBar();
    }

    private void ResetHealthBar()
    {
        foreach (Transform child in healthBar)
            Destroy(child.gameObject);

        for (int i = 0; i < playerRef.MaxHealth; ++i)
            Instantiate(healthPoint, healthBar);
    }

    public void DecreaseHealthBar(int number)
    {
        int usedValue = number > healthBar.childCount ? healthBar.childCount : number;

        for (int i = 0; i != usedValue; ++i)
            Destroy(healthBar.GetChild(0).gameObject);
    }

    public void UpdateScore()
    {
        scoreTracker = ScoreManager.Score;
        scoreText.text = scoreTracker.ToString("000000000000");
    }

    private void Update()
    {
        if (ScoreManager.Score != scoreTracker)
            UpdateScore();
    }
}
