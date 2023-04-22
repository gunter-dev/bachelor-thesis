using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class MainMenuController : MonoBehaviour
    {
        public Light2D menuLight;
        
        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeLightColor(string colorName)
        {
            var property = typeof(Color).GetProperty(colorName);
            if (property != null) menuLight.color = (Color)property.GetValue(null, null);
        }

        public void StartLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
