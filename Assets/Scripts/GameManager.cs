using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Manager Components")]
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;
    private ProgressBar progressBar;

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
    }

    private void Start()
    {
        progressBar = FindFirstObjectByType<ProgressBar>();
        
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void PauseToggle()
    {
        isPaused = !isPaused;

        if (isPaused)
            PauseGame();
        else
            UnpauseGame();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);

        progressBar.GetComponent<DrawerGUIMovement>().TakeOutDrawer();
    }

    // This method is public for the button to unpause within the main menu of the pause screen
    /// <summary>
    /// 
    /// </summary>
    public void UnpauseGame()
    {
        progressBar.GetComponent<DrawerGUIMovement>().PushInDrawer();

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
