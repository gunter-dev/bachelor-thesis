using SFB;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class FileSelectorController : MonoBehaviour
    {
        public void OpenFileSelector(bool createLevelMode)
        {
            var extensions = new [] {
                new ExtensionFilter("TIF Files", "tif", "tiff"),
            };
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

            if (path.Length == 0) return;
            
            GlobalVariables.pathToLevel = path[0];
            GlobalVariables.createLevelMode = createLevelMode;
            SceneManager.LoadScene(createLevelMode ? Constants.CreateLevelScene : Constants.LocalLevel);
        }
    }
}
