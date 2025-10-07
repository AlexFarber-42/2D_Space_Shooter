using UnityEngine;
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
        GameManager.Instance.DestroyBackground();
        SceneManager.LoadScene(0);
    }

    public void UnpauseCall()
        => Player.Instance.UnpauseGame();
}
