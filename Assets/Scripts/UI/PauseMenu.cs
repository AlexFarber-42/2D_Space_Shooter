using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ReturnToTitleScreen()
    {
        //  TODO ---> Add further functionality, i.e. for saving and such
        SceneManager.LoadScene(0);
    }
}
