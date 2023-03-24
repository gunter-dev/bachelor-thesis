using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class ErrorScreen : MonoBehaviour
    {
        public TMP_Text errorText;
    
        private void Start()
        {
            errorText.text = GlobalVariables.errorMessage;
        }

        public void Back()
        {
            SceneManager.LoadScene(Constants.MainMenu);
        }

        public void Reload()
        {
            SceneManager.LoadScene(Constants.CreateLevelScene);
        }
    }
}
