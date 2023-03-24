using SFB;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class CreateLevelController : MonoBehaviour
    {
        public void OpenFileSelector()
        {
            var extensions = new [] {
                new ExtensionFilter("TIF Files", "tif", "tiff"),
            };
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

            GlobalVariables.pathToLevel = path[0];
            GlobalVariables.createLevelMode = true;
            SceneManager.LoadScene(Constants.CreateLevelScene);
        }
    }
}
