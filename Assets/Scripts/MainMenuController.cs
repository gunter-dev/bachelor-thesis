using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Scenes/LobbyScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
