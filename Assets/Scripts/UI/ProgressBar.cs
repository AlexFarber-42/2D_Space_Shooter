using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("Progress Bar Properties")]
    [SerializeField]
    private RectTransform container;

    [SerializeField]
    [Tooltip("Manual points for the progress bar to show, put in decimal format, i.e. 40% --> .4")] 
    private float[] thresholdsToShow;
    private int currentThreshIndex = 0;

    private GameObject progressPoint;
    private int progressPointMax = 0;

    private RectTransform rect;

    private bool levelActive = false;

    public void ToggleLevelActivity()
        => levelActive = !levelActive;

    private void Awake()
    {
        // Set the width and height of the progress bar to a more uniform size
        rect = GetComponent<RectTransform>();

        float width = Screen.width / 3;
        float height = Screen.height / 20;

        rect.sizeDelta = new Vector2(width, height);

        progressPoint = Resources.Load<GameObject>("Prefabs/UI/Progress_Point");

        CalculateProgressPointLimit();
        currentThreshIndex = 0;
    }

    /// <summary>
    /// Calculator for the maximum amount of progress points that can be indicated on the screen
    /// </summary>
    private void CalculateProgressPointLimit()
    {
        // Calculates the starting point of the rect transform for the actual transform that holds the progress points
        // Should always be positive
        float curPos = (rect.sizeDelta.x - container.rect.width) / 2;

        if (curPos <= 0)
        {
            Debug.LogError($"curPos in the CalculateProgressPointLimit() in ProgressBar.cs indicates an impossible non-positive value: {curPos}");
            progressPointMax = 1;
            return;
        }

        // Value represents the spacing between each progress point, can be negative or
        // positive depending on the user's desire for the points to be close or spaced apart
        float spacingFix = container.GetComponent<HorizontalLayoutGroup>().spacing;

        // Sets the limit to the other side of the container
        float progressPointWidth = progressPoint.GetComponent<LayoutElement>().preferredWidth;
        float delta = progressPointWidth + spacingFix;
        float posLimit = rect.sizeDelta.x - curPos - delta;

        // Increment to next point check until the end is reached
        while (curPos < posLimit)
        {
            ++progressPointMax;
            curPos += delta;
        }
    }

    private WaveManager waveMan;

    private void Start()
    {
        waveMan = WaveManager.Instance;
    }

    private void Update()
    {
        if (!levelActive)
            return;

        if (waveMan.WaveComplete)
        {
            ToggleLevelActivity();
            GameManager.Instance.SignalLevelComplete();
            return;
        }

        // Add any progress points if necessary
        if (container.childCount / (float)progressPointMax < waveMan.EnemyProgress)
            AddProgressPoint();

        CheckThresholds();
    }

    private void CheckThresholds()
    {
        if (thresholdsToShow.Length is 0 || currentThreshIndex == thresholdsToShow.Length)
            return;

        // When a threshold is met, show the drawer and increment to the next threshold
        if (thresholdsToShow[currentThreshIndex] <= waveMan.EnemyProgress)
        {
            GetComponent<DrawerGUIMovement>().TriggerSlip();
            currentThreshIndex++;
        }
    }

    private void AddProgressPoint()
    {
        if (container.childCount == progressPointMax)
        {
            Debug.LogWarning("Attempting to add another progress point despite reaching max amount in AddProgressPoint() in ProgressBar.cs");
            return;
        }

        Instantiate(progressPoint, container);
    }

    public void Reset()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
}
