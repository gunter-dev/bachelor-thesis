using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;

using BitMiracle.LibTiff.Classic;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using GameObject = UnityEngine.GameObject;

public class LevelGenerator : MonoBehaviour
{
    private Tiff _levelImage;
    private string _pathToLevelImage;

    private int _mapWidth;
    private int _mapHeight;
    private int[] _raster;

    private GameObject _player;
    private int _keysAmount;

    private ColorPrefab[] _colorMappings;

    public TMP_Text warningText;

    // Start is called before the first frame update
    public void Start()
    {
        InitializePath();

        InstantiateColorMappings();
        ImportImageFromFile();
        GenerateLevel();

        SpawnCamera();
    }

    void ReloadLevel()
    {
        GlobalVariables.pathToLevel = _pathToLevelImage;
        SceneManager.LoadScene(Constants.CreateLevelScene);
    }

    private void InitializePath()
    {
        _pathToLevelImage = GlobalVariables.pathToLevel ?? GetFile("Assets/first-test-level-69.tif");
        GlobalVariables.pathToLevel = null;
    }
    
    private void InstantiateColorMappings()
    {
        _colorMappings = new ColorPrefab[]
        {
            new (Color.FromArgb(0, 0, 0), "Grounds/Grass Ground"),
            new (Color.FromArgb(255, 0, 0), "Player"),
            new (Color.FromArgb(255, 255, 255), "Grounds/Accelerator"),
            new (Color.FromArgb(0, 255, 0), "Grounds/Slime"),
            new (Color.FromArgb(0, 255, 255), "Grounds/Ice"),
            new (Color.FromArgb(255, 0, 255), "Spike"),
            new (Color.FromArgb(0, 0, 255), "Grounds/Gravity"),
            new (Color.FromArgb(255, 255, 0), "Grounds/Fan"),
            new (Color.FromArgb(100, 0, 0), "Grounds/Movable Grass"),
            new (Color.FromArgb(100, 100, 0), "Grounds/Disappearing Ground"),
            new (Color.FromArgb(100, 100, 100), "Exit")
        };
    }

    private void ImportImageFromFile()
    {
        _levelImage = Tiff.Open(_pathToLevelImage, "r");

        if (_levelImage != null)
        {
            _mapWidth = _levelImage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            _mapHeight = _levelImage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            GlobalVariables.mapWidth = _mapWidth;
            GlobalVariables.mapHeight = _mapHeight;

            int imageSize = _mapWidth * _mapHeight;
            _raster = new int[imageSize];
        } else DisplayError("Error opening file: '" + _pathToLevelImage + "'. Please try a different file.");
    }

    private void GenerateLevel()
    {
        for (short directory = 0; directory < _levelImage.NumberOfDirectories(); ++directory)
        {
            _levelImage.SetDirectory(directory);
            // Read the image into the memory buffer
            _levelImage.ReadRGBAImage(_mapWidth, _mapHeight, _raster);

            string pageName = _levelImage.GetField(TiffTag.PAGENAME)[0].ToString();
            switch (pageName)
            {
                case Constants.MainLayer:
                    GenerateMain();
                    break;
                case Constants.ElectricityLayer:
                    GenerateElectricity();
                    break;
                case Constants.MovingPlatformLayer:
                    GenerateMovingPlatforms();
                    break;
                case Constants.KeysLayer:
                    GenerateKeys();
                    break;
                case Constants.LightLayer:
                    GenerateLights();
                    break;
                default: 
                    DisplayWarning("Unknown layer name: '" + pageName + "'. This layer was ignored.");
                    break;
            }
        }

        GenerateGlobalLight();
        _levelImage.Close();
    }

    private void GenerateMain()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = GetPixelColor(x, y);

        int flippedY = _mapHeight - 1 - y;

        // The pixel is transparent, we want to ignore it.
        if (pixelColor.A == 0) return;

        foreach (ColorPrefab colorMapping in _colorMappings)
        {
            if (pixelColor.Equals(Color.FromArgb(0, 0, 0)))
            {
                /*bool leftBottom, leftTop, rightBottom, rightTop = false;
                if (x != 0)
                {
                    if (y != _mapHeight) leftTop = GetPixelColor(x - 1, y + 1).A != 0;
                    else if (y != 0)      leftBottom = GetPixelColor(x - 1, y - 1).A != 0;
                }

                if (x != _mapHeight)
                {
                    if (y != _mapHeight) rightTop = GetPixelColor(x + 1, y + 1).A != 0;
                    else if (y != 0)      rightBottom = GetPixelColor(x + 1, y - 1).A != 0;
                }
                
                bool left =   x != 0           && GetPixelColor(x - 1, y).A != 0;
                bool right =  y != _mapWidth  && GetPixelColor(x + 1, y).A != 0;
                bool bottom = y != 0           && GetPixelColor(x, y - 1).A != 0;
                bool top =    x != _mapHeight && GetPixelColor(x, y + 1).Equals(Color.Black);

                if (!top)         Instantiate(groundPrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
                else if (!right)  Instantiate(leftSidePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
                else if (!left)   Instantiate(rightSidePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
                else if (!bottom) Instantiate(topPrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
                else */
                Spawn("Grounds/Grass Ground", new Vector3(x, flippedY, 1));
                return;
            }

            if (!colorMapping.color.Equals(pixelColor)) continue;
            
            GameObject block = Spawn(colorMapping.pathToPrefab, new Vector3(x, flippedY, 1));
            if (block.CompareTag(Constants.FanTag))
            {
                Spawn("Fan Area Effector", new Vector3(x, flippedY + 1, 1));
                Spawn("Fan Area Effector", new Vector3(x, flippedY + 2, 1));
            }
            else if (block.CompareTag(Constants.PlayerTag))
            {
                _player = block;
            }

            return;
        }

        DisplayWarning("Main layer - (" + x + ", " + y + "): There is an invalid color on these coordinates. This pixel has been ignored.");
    }

    private void GenerateElectricity()
    {
        var blocks = new Dictionary<int, List<ElectricityInfo>>();
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);

                if (pixelColor.A == 0) continue;
                
                int flippedY = _mapHeight - 1 - y;

                if (blocks.ContainsKey(pixelColor.R)) blocks[pixelColor.R].Add(new ElectricityInfo(x, flippedY, pixelColor.B));
                else blocks.Add(pixelColor.R, new List<ElectricityInfo> { new (x, flippedY, pixelColor.B) });
            }
        }

        foreach (var block in blocks)
        {
            block.Value.Sort((l, r) => l.colorCode.CompareTo(r.colorCode));

            var firstCoordinate = block.Value.Aggregate((l, r) => l.colorCode < r.colorCode ? l : r);

            GameObject buttonObject = Spawn("Grounds/Button", new Vector3(firstCoordinate.x, firstCoordinate.y, 1));
            buttonObject.GetComponent<ButtonController>().affectedBlocks = block.Value;
        }
    }

    private void GenerateMovingPlatforms()
    {
        var platforms = new Dictionary<int, List<PlatformInfo>>();
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);

                if (pixelColor.A == 0) continue;
                
                int flippedY = _mapHeight - 1 - y;

                if (platforms.ContainsKey(pixelColor.R)) platforms[pixelColor.R].Add(new PlatformInfo(x, flippedY, pixelColor.B, pixelColor.G));
                else platforms.Add(pixelColor.R, new List<PlatformInfo> { new (x, flippedY, pixelColor.B, pixelColor.G) });
            }
        }

        foreach (var platform in platforms)
        {
            platform.Value.Sort((l, r) => l.colorCode.CompareTo(r.colorCode));

            PlatformInfo first = platform.Value[0];

            GameObject platformObject = Spawn("Grounds/Moving Platform", new Vector3(first.x, first.y, 1));
            platformObject.GetComponent<MovingPlatformController>().path = platform.Value;
        }
    }

    private void GenerateKeys()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);

                if (pixelColor.A == 0) continue;

                int flippedY = _mapHeight - 1 - y;
                
                if (pixelColor.Equals(Color.FromArgb(0, 0, 0)))
                {
                    Spawn("Key", new Vector3(x, flippedY, 1));
                    _keysAmount++;
                }
                else if (pixelColor.Equals(Color.FromArgb(255, 255, 255)))
                {
                    Spawn("Grounds/Key Hole", new Vector3(x, flippedY, 1));
                }
                else
                {
                    DisplayWarning("Keys layer - (" + x + ", " + y + "): There is an invalid color on these coordinates. This pixel has been ignored.");
                }
            }
        }

        _player.GetComponent<Player>().keysNeeded = _keysAmount;
    }

    private void GenerateLights()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);

                if (pixelColor.A == 0) continue;

                int flippedY = _mapHeight - 1 - y;

                Light2D spotLight = Resources.Load<Light2D>("Spot Light");
                spotLight.color = ParseToUnityColor(pixelColor);

                Instantiate(spotLight, new Vector2(x, flippedY), Quaternion.identity);
            }
        }
    }

    private void GenerateGlobalLight()
    {
        Instantiate(GetPrefab("Global Light"), Vector3.zero, Quaternion.identity);
    }

    private void SpawnCamera()
    {
        // z is -10 because the camera needs to be position "in front" of everything else
        Vector3 cameraPosition = new Vector3(_mapWidth / 2f + Constants.MapStartingCoordinate, _mapHeight / 2f  + Constants.MapStartingCoordinate, -10);

        Camera mainCamera = Resources.Load<Camera>("Main Camera");
        mainCamera = Instantiate(mainCamera, cameraPosition, Quaternion.identity);
        
        mainCamera.orthographicSize = _mapHeight / 2f;
    }

    private Color GetPixelColor(int x, int y)
    {
        int offset = (_mapHeight - y - 1) * _mapWidth + x;
        int alpha = Tiff.GetA(_raster[offset]);
        int red = Tiff.GetR(_raster[offset]);
        int green = Tiff.GetG(_raster[offset]);
        int blue = Tiff.GetB(_raster[offset]);
        return Color.FromArgb(alpha, red, green, blue);
    }

    private UnityEngine.Color ParseToUnityColor(Color srcColor)
    {
        return new UnityEngine.Color(srcColor.R / 255f, srcColor.G / 255f, srcColor.B / 255f, srcColor.A / 255f);
    }

    private static GameObject GetPrefab(string pathName)
    {
        return Resources.Load<GameObject>(pathName);
    }

    private static GameObject Spawn(string pathName, Vector3 position)
    {
        return Instantiate(GetPrefab(pathName), position, Quaternion.identity);
    }

    private static string GetFile(string fileName)
    {
        return Directory.GetCurrentDirectory() + "/" + fileName;
    }

    private void DisplayWarning(string warning)
    {
        if (GlobalVariables.createLevelMode) warningText.text = warning;
    }

    private void DisplayError(string error)
    {
        GlobalVariables.errorMessage = error;
        SceneManager.LoadScene(Constants.ErrorScene);
    }

    // ReSharper disable once UnusedMember.Local
    private void LogObject(Color obj)
    {
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
        {
            string propName = descriptor.Name;
            object value = descriptor.GetValue(obj);
            Debug.Log(propName + ": " + value);
        }
    }
}
