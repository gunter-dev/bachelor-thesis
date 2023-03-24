using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
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
}
