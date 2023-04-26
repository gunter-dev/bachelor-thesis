using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Linq;
using Color = System.Drawing.Color;

using BitMiracle.LibTiff.Classic;
using BlockScripts;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using GameObject = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private Tiff _levelImage;

    private int _mapWidth;
    private int _mapHeight;
    private int[] _raster;

    private bool _lightLayerPresent;
    private bool _exitSpawned;
    private bool _playerSpawned;

    private ColorPrefab[] _colorMappings;

    public TMP_Text warningText;

    public void Start()
    {
        InstantiateColorMappings();
        ImportImageFromFile();
        GenerateLevel();
        RenderBackground();
        SpawnCamera();
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(Constants.CreateLevelScene);
    }

    private void InstantiateColorMappings()
    {
        _colorMappings = new ColorPrefab[]
        {
            new (Color.FromArgb(0, 0, 0), "Grounds/Grass Ground"),
            new (Color.FromArgb(255, 0, 0), "Player"),
            new (Color.FromArgb(255, 255, 255), "Grounds/Accelerator"),
            new (Color.FromArgb(0, 255, 0), "Grounds/Slime"),
            new (Color.FromArgb(255, 0, 255), "Spike"),
            new (Color.FromArgb(0, 0, 255), "Grounds/Gravity"),
            new (Color.FromArgb(100, 0, 0), "Grounds/Box"),
            new (Color.FromArgb(100, 100, 0), "Grounds/Disappearing Ground"),
            new (Color.FromArgb(100, 100, 100), "Exit")
        };
    }

    private void ImportImageFromFile()
    {
        _levelImage = Tiff.Open(GlobalVariables.pathToLevel, "r");

        if (_levelImage != null)
        {
            _mapWidth = _levelImage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            _mapHeight = _levelImage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            GlobalVariables.mapWidth = _mapWidth;
            GlobalVariables.mapHeight = _mapHeight;

            int imageSize = _mapWidth * _mapHeight;
            _raster = new int[imageSize];
        } else DisplayError(GlobalVariables.pathToLevel + Messages.FileOpeningError);
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
                    _lightLayerPresent = true;
                    GenerateLights();
                    break;
                default: 
                    DisplayWarning(Messages.LayerError(pageName));
                    break;
            }
        }

        GenerateGlobalLight();
        Physics2D.gravity = new Vector2(0, -9.8f);
        _levelImage.Close();
        
        if (!_exitSpawned) DisplayWarning(Messages.MissingExit);
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
        
        if (!_playerSpawned) DisplayError(Messages.PlayerNotSpawned);
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = GetPixelColor(x, y);
        if (pixelColor.A < 255) return;

        if (pixelColor is { R: 0, G: 0, B: < 4 })
        {
            Spawn(GetRegularBlock(pixelColor.B), new Vector2(x, GetFlippedY(y)));
            return;
        }

        foreach (ColorPrefab colorMapping in _colorMappings)
        {
            if (!colorMapping.color.Equals(pixelColor)) continue;

            if (colorMapping.pathToPrefab == "Exit")
            {
                SpawnExit(x, y);
                return;
            }

            GameObject block = Spawn(colorMapping.pathToPrefab, new Vector3(x, GetFlippedY(y), 1));
            
            if (block.CompareTag(Constants.GravityBlockTag))
            {
                if (GetFlippedY(y) == 0) return;
                if (!IsPixelEmpty(x, y - 1)) FlipBlockOnY(block);
            }
            else if (block.CompareTag(Constants.PlayerTag))
            {
                if (_playerSpawned) DisplayError(Messages.MultipleCharacters);
                if (!IsPixelEmpty(x, y - 1)) {
                    DisplayError(Messages.PlayerCannotBeSpawned);
                    return;
                }
                _playerSpawned = true;
            }

            return;
        }

        DisplayWarning(Messages.LayerCoordinatesWarning("Main layer", x, y));
    }

    private string GetRegularBlock(int blue)
    {
        int randomValue;
        switch (blue)
        {
            case 0:
                randomValue = Random.Range(1, 4);
                return "Grounds/Grass Ground " + randomValue;
            case 1:
                randomValue = Random.Range(1, 5);
                return "Walls/Left Wall " + randomValue;
            case 2:
                randomValue = Random.Range(1, 5);
                return "Walls/Right Wall " + randomValue;
            case 3:
                randomValue = Random.Range(1, 3);
                return "Grounds/Top " + randomValue;
        }
        return "Grounds/Grass Ground 1";
    }

    private void SpawnExit(int x, int y)
    {
        if (_exitSpawned) DisplayError(Messages.MultipleExits);

        float resultX = x;
        float resultY = y;

        if (!IsPixelEmpty(x + 1, y))
        {
            if (!IsPixelEmpty(x - 1, y)) DisplayError(Messages.NotEnoughSpaceForExit);
            resultX += Constants.MapStartingCoordinate;
        }
        
        if (!IsPixelEmpty(x, y + 1))
        {
            if (!IsPixelEmpty(x, y - 1)) DisplayError(Messages.NotEnoughSpaceForExit);
            resultY += Constants.MapStartingCoordinate;
        }

        float flippedY = _mapHeight - 1 - resultY;
        Spawn("Exit", new Vector2(resultX, flippedY));
        
        _exitSpawned = true;
    }

    private void FlipBlockOnY(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().flipY = true;
        BoxCollider2D col = block.GetComponent<BoxCollider2D>();
        Vector2 offset = col.offset;
        offset.y *= -1;
        col.offset = offset;
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

                blocks.TryAdd(pixelColor.R, new List<ElectricityInfo>());
                blocks[pixelColor.R].Add(new ElectricityInfo(x, GetFlippedY(y), pixelColor.B));
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
        int size = 0;
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);
                if (pixelColor.A == 0) continue;

                platforms.TryAdd(pixelColor.R, new List<PlatformInfo>());
                platforms[pixelColor.R].Add(new PlatformInfo(x, GetFlippedY(y), pixelColor.B));

                if (size == 0) size = pixelColor.G;
            }
        }
        
        if (size == 0) DisplayError("Invalid platform size! The size cannot be zero!");

        foreach (var platform in platforms)
        {
            platform.Value.Sort((l, r) => l.colorCode.CompareTo(r.colorCode));

            PlatformInfo first = platform.Value[0];

            GameObject platformObject = Spawn("Grounds/Moving Platform", new Vector3(first.x, first.y, 1));

            List<Vector2> path = new List<Vector2>();

            foreach (var point in platform.Value)
                path.Add(new Vector2(point.x, point.y));

            MovingPlatformController controller = platformObject.GetComponent<MovingPlatformController>();
            controller.path = path;
            controller.size = size;
        }
    }

    private void GenerateKeys()
    {
        var doors = new List<DoorInfo>();
        var keys = new Dictionary<int, List<Coordinates>>();
        
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);
                if (pixelColor.A == 0) continue;

                if (pixelColor is { R: 0, G: 0, B: 0 })
                {
                    keys.TryAdd(pixelColor.A, new List<Coordinates>());
                    keys[pixelColor.A].Add(new Coordinates(x, GetFlippedY(y)));
                }
                else
                {
                    bool aboveEmpty = IsPixelEmpty(x, y + 1);
                    bool belowEmpty = IsPixelEmpty(x, y - 1);
                    
                    if (aboveEmpty && belowEmpty) DisplayError(Messages.InvalidDoor);
                    
                    else if (!aboveEmpty && !belowEmpty)
                    {
                        Color aboveColor = GetPixelColor(x, y + 1);
                        Color belowColor = GetPixelColor(x, y - 1);
                        
                        if (aboveColor.A  == belowColor.A && !aboveColor.Equals(Color.Black))
                            DisplayError(Messages.InvalidDoor);
                    }
                    
                    else if (belowEmpty && GetPixelColor(x, y + 1).A == pixelColor.A)
                    {
                        if (doors.Find(door => door.groupId == pixelColor.A) != null)
                            DisplayError(Messages.MultipleSameColoredDoors);
                        
                        doors.Add(new DoorInfo(x, GetFlippedY(y), pixelColor.A, pixelColor));
                    }
                    
                    else if (!(aboveEmpty && GetPixelColor(x, y - 1).A == pixelColor.A)) 
                        DisplayError(Messages.InvalidDoor);
                }
            }
        }

        if (doors.Count < keys.Count) DisplayWarning(Messages.KeysWithoutKeyHoleWarning);

        foreach (var door in doors)
        {
            GameObject holeObject = Spawn("Grounds/Door", new Vector2(door.x, door.y - 0.5f));
            
            if (!keys.ContainsKey(door.groupId))
                DisplayWarning(Messages.KeyHoleWithoutKeysWarning(door.x, door.y));
            else
                holeObject.GetComponent<DoorController>().keys = keys[door.groupId];

            UnityEngine.Color lightColor = ParseToUnityColorForKeys(door.lightColor);
            lightColor.a = 1;
            holeObject.GetComponent<DoorController>().lightColor = lightColor;
        }
    }

    private void GenerateLights()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);
                if (pixelColor.A == 0) continue;

                Light2D spotLight = Resources.Load<Light2D>("Spot Light");
                spotLight.color = ParseToUnityColor(pixelColor);

                Instantiate(spotLight, new Vector2(x, GetFlippedY(y)), Quaternion.identity);
            }
        }
    }

    private void GenerateGlobalLight()
    {
        Light2D globalLight = Instantiate(Resources.Load<Light2D>("Global Light"), Vector3.zero, Quaternion.identity);
        if (!_lightLayerPresent) globalLight.intensity = 0.6f;
    }
    
    private void RenderBackground()
    {
        GameObject background = Resources.Load<GameObject>("ExtendableBackground");
        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        Vector3 backgroundSize = backgroundSpriteRenderer.bounds.size;

        Vector2 position = new Vector2(
            (backgroundSize.x / 2) + Constants.MapStartingCoordinate,
            (backgroundSize.y / 2) + Constants.MapStartingCoordinate
        );

        while (position.x - backgroundSize.x / 2 < GlobalVariables.mapWidth)
        {
            while (position.y - backgroundSize.y / 2 < GlobalVariables.mapHeight)
            {
                Instantiate(background, position, Quaternion.identity);
                position.y += backgroundSize.y;
            }
            
            position.x += backgroundSize.x;
            position.y = (backgroundSize.y / 2) + Constants.MapStartingCoordinate;
        }
    }   

    private void SpawnCamera()
    {
        // z is -10 because the camera needs to be position "in front" of everything else
        Vector3 cameraPosition = new Vector3(_mapWidth / 2f + Constants.MapStartingCoordinate, _mapHeight / 2f  + Constants.MapStartingCoordinate, -10);

        Camera mainCamera = Resources.Load<Camera>("Main Camera");
        mainCamera = Instantiate(mainCamera, cameraPosition, Quaternion.identity);

        float mapAspectRatio = (float)_mapWidth / _mapHeight;
        float screenAspectRatio = (float)Screen.width / Screen.height;
        
        mainCamera.orthographicSize = (_mapHeight / 2f) * (mapAspectRatio / screenAspectRatio);
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

    private bool IsPixelEmpty(int x, int y)
    {
        if (x < 0 || y < 0 || x > _mapWidth - 1 || y > _mapHeight - 1) return false;
        return GetPixelColor(x, y).A == 0;
    }

    private UnityEngine.Color ParseToUnityColor(Color srcColor)
    {
        return new UnityEngine.Color(srcColor.R / 255f, srcColor.G / 255f, srcColor.B / 255f, srcColor.A / 255f);
    }
    
    private UnityEngine.Color ParseToUnityColorForKeys(Color srcColor)
    {
        return new UnityEngine.Color((float)srcColor.R / srcColor.A, (float)srcColor.G / srcColor.A, (float)srcColor.B / srcColor.A, 1);
    }

    private int GetFlippedY(int y)
    {
        return _mapHeight - 1 - y;
    }

    private static GameObject GetPrefab(string pathName)
    {
        return Resources.Load<GameObject>(pathName);
    }

    private static GameObject Spawn(string pathName, Vector3 position)
    {
        return Instantiate(GetPrefab(pathName), position, Quaternion.identity);
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
