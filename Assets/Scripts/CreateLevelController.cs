using UnityEditor;
using UnityEngine;

public class CreateLevelController : MonoBehaviour
{
    private string _selectedFilePath;
    
    public void OpenFileSelector()
    {
        _selectedFilePath = EditorUtility.OpenFilePanel("Select a TIFF image", "", "tif");

        GameObject gameObject = new GameObject("LevelGenerator");
        LevelGenerator generator = gameObject.AddComponent<LevelGenerator>();
        generator.SetPathToLevelImage(_selectedFilePath);
        
        generator.Start();
    }
}
