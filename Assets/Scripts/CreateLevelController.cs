using UnityEngine;
using AnotherFileBrowser.Windows;
using UnityEngine.SceneManagement;

public class CreateLevelController : MonoBehaviour
{
    public void OpenFileSelector()
    {
        var bp = new BrowserProperties
        {
            title = "Select a TIF image",
            initialDir = "",
            filter = "TIF files (*.tif)|*.tif",
            filterIndex = 0
        };

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            GlobalVariables.pathToLevel = path;
            GlobalVariables.createLevelMode = true;
            SceneManager.LoadScene(Constants.CreateLevelScene);
        });
    }
}
