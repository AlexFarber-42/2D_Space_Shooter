using System;
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

    public void IncreaseHealthBar(int number)
    {
        for (int i = 0; i < number; ++i)
            Instantiate(healthPoint, healthBar);
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

    public void UpdatePowerup(GameObject powerupProj)
    {
        powerUpImage.sprite = powerupProj.GetComponent<SpriteRenderer>().sprite;
        powerUpImage.type = Image.Type.Simple;
        powerUpImage.preserveAspect = true;

        powerUpImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
    }
}
