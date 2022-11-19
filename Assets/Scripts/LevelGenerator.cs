using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Linq;
using Color = System.Drawing.Color;

using BitMiracle.LibTiff.Classic;

public class LevelGenerator : MonoBehaviour
{
    private const string MainLayer = "main";
    private const string ElectricityLayer = "electricity";
    private const string MovingPlatformLayer = "movingPlatforms";
    private const string KeysLayer = "keys";

    private Tiff _levelImage;
    
    private int _mapWidth;
    private int _mapHeight;
    private int[] _raster;
    
    [SerializeField] 
    private GameObject backgroundImage;

    private ColorPrefab[] _colorMappings;

    public Camera cameraPrefab;
    public GameObject fanAreaEffectorPrefab;
    public GameObject groundPrefab, rightSidePrefab, leftSidePrefab, topPrefab,
        playerPrefab, acceleratorPrefab, fanPrefab, gravityPrefab, icePrefab,
        slimePrefab, spikePrefab, movablePrefab, disappearingPrefab, movingPlatformPrefab;

    private const string FanTag = "Fan";

    // Start is called before the first frame update
    void Start()
    {
        InstantiateColorMappings();
        ImportImageFromFile();
        GenerateLevel();
        
        SpawnCamera();
        RenderBackground();
    }

    void InstantiateColorMappings()
    {
        _colorMappings = new ColorPrefab[]
        {
            new (Color.FromArgb(0, 0, 0), groundPrefab),
            new (Color.FromArgb(255, 0, 0), playerPrefab),
            new (Color.FromArgb(255, 255, 255), acceleratorPrefab),
            new (Color.FromArgb(0, 255, 0), slimePrefab),
            new (Color.FromArgb(0, 255, 255), icePrefab),
            new (Color.FromArgb(255, 0, 255), spikePrefab),
            new (Color.FromArgb(0, 0, 255), gravityPrefab),
            new (Color.FromArgb(255, 255, 0), fanPrefab),
            new (Color.FromArgb(100, 0, 0), movablePrefab),
            new (Color.FromArgb(100, 100, 0), disappearingPrefab)
        };
    }

    void ImportImageFromFile()
    {
        _levelImage = Tiff.Open("C:\\Users\\lucky\\Bachelor Thesis\\Assets\\first-test-level.tif", "r");
        _mapWidth = _levelImage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
        _mapHeight = _levelImage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
        
        int imageSize = _mapWidth * _mapHeight;
        _raster = new int[imageSize];
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
                case MainLayer:
                    GenerateMain();
                    break;
                case ElectricityLayer:
                    break;
                case MovingPlatformLayer:
                    GenerateMovingPlatforms();
                    break;
                case KeysLayer:
                    GenerateKeys();
                    break;
            }
        }
    }

    void GenerateMain()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    void GenerateTile(int x, int y)
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
                Instantiate(groundPrefab, new Vector3(x, flippedY, 1), Quaternion.identity, transform);
            }
            else if (colorMapping.color.Equals(pixelColor))
            {
                if (colorMapping.prefab.CompareTag(FanTag))
                {
                    Instantiate(fanAreaEffectorPrefab, new Vector3(x, flippedY + 1, 1), Quaternion.identity);
                    Instantiate(fanAreaEffectorPrefab, new Vector3(x, flippedY + 2, 1), Quaternion.identity);
                }
                
                Vector3 position = new Vector3(x, flippedY, 1);
                Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
            }
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
            var firstCoordinate = platform.Value.Aggregate((l, r) => l.colorCode < r.colorCode ? l : r);
            
            GameObject platformObject = Instantiate(movingPlatformPrefab, new Vector3(firstCoordinate.x, firstCoordinate.y, 1), Quaternion.identity);
            platformObject.GetComponent<MovingPlatformController>().path = platform.Value;
            platformObject.GetComponent<MovingPlatformController>().platformPrefab = movingPlatformPrefab;
        }
    }

    void GenerateKeys()
    {
        int keysAmount = 0;
        
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Color pixelColor = GetPixelColor(x, y);

                if (pixelColor.A == 0) continue;

                if (pixelColor.Equals(Color.FromArgb(0, 0, 0)))
                {
                    // TODO: hell yeah
                    keysAmount++;
                }
                
                int flippedY = _mapHeight - 1 - y;
            }
        }
    }

    void SpawnCamera()
    {
        // z is -10 because the camera needs to be position "in front" of everything else
        Vector3 cameraPosition = new Vector3((_mapWidth / 2f) - 0.5f, (_mapHeight / 2f) - 0.5f, -10);
        
        Camera mainCamera = Instantiate(cameraPrefab, cameraPosition, Quaternion.identity);
        mainCamera.orthographicSize = _mapHeight / 2f;
    }

    void RenderBackground()
    {
        Vector3 position = new Vector3(_mapWidth / 2f, _mapHeight / 2f, 0);
        GameObject background = Instantiate(backgroundImage, position, Quaternion.identity);
        background.transform.localScale = new Vector2(7, 7);
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

    void LogObject(Color obj)
    {
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
        {
            string propName = descriptor.Name;
            object value = descriptor.GetValue(obj);
            Debug.Log(propName + ": " + value);
        }
    }
}
