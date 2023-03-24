using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Play()
    {
        GlobalVariables.createLevelMode = false;
        SceneManager.LoadScene(Constants.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
