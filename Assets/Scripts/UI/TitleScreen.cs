using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void OnNewGame()
    {
        // TODO ---> Extend method eventually, as 1 is just the TestScene
        SceneManager.LoadScene(1);
    }

    public void OnLoadGame()
    {

    }

    public void ExitApplication()
    {
        // TODO ---> Most likely need further functionality before quitting
        Application.Quit();
    }
}
