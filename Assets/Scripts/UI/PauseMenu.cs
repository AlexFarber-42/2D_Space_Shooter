using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseObject;

    public void ActivatePause()
        => pauseObject.SetActive(true);
    public void DeactivatePause()
        => pauseObject.SetActive(false);

    public void ReturnToTitleScreen()
    {
        //  TODO ---> Add further functionality, i.e. for saving and such
        SceneManager.LoadScene(0);
    }

    public void UnpauseCall()
        => FindFirstObjectByType<Player>().UnpauseGame();
}
