using UnityEngine;
using Color = UnityEngine.Color;

public class MainCamera : MonoBehaviour
{
    private Transform _playerPosition;
    private Vector3 _cameraPosition;
    
    private const float MapStartingCoordinate = -0.5f;
    private const float AspectRatio = 16f / 9f; // 16:9 is the used aspect ratio
    private const float CameraSizeChange = 0.3f;
    private const float MaxRedTintOpacity = 0.7f;
    private const float AppearingTime = 2;
    
    private float _defaultCameraSize;
    private float _currentCameraSize;

    private Camera _camera;

    private bool _deathScreenVisible;

    public float mapHeight;
    public float mapWidth;

    public GameObject deathScreenTintPrefab;
    private SpriteRenderer _deathScreenTintSpriteRenderer;

    void Start()
    {
        _playerPosition = GameObject.FindWithTag("Player").transform;
        _camera = GetComponent<Camera>();
        
        // The orthographicSize is half the size of the vertical viewing volume. -> https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html
        // At the Start, the orthographic size of camera is the same, as the half of map height
        _defaultCameraSize = _camera.orthographicSize;
        
        RenderBackground();
    }

    void LateUpdate()
    {
        if (!_playerPosition)
        {
            if (!_deathScreenVisible) InstantiateRedTint();
            UpdateRedTintOpacity(_deathScreenTintSpriteRenderer.color.a + Time.deltaTime / AppearingTime);
            return;
        }
        
        ChangeSizeOnScroll();
        FocusPlayer();

        _currentCameraSize = _camera.orthographicSize;
    }

    private void InstantiateRedTint()
    {
        Vector3 position = new Vector3(_cameraPosition.x, _cameraPosition.y, 0);
        
        GameObject deathScreenTint = Instantiate(deathScreenTintPrefab, position, Quaternion.identity);

        _deathScreenTintSpriteRenderer = deathScreenTint.GetComponent<SpriteRenderer>();
        Vector3 tintSize = _deathScreenTintSpriteRenderer.bounds.size;
        
        deathScreenTint.transform.localScale = new Vector2(2 * _currentCameraSize * AspectRatio / tintSize.x, 2 * _currentCameraSize / tintSize.y);

        UpdateRedTintOpacity(0);

        _deathScreenVisible = true;
    }

    private void UpdateRedTintOpacity(float opacity)
    {
        if (opacity <= MaxRedTintOpacity)
        {
            Color deathScreenColor = _deathScreenTintSpriteRenderer.color;
            deathScreenColor.a = opacity;
            _deathScreenTintSpriteRenderer.color = deathScreenColor;
        }
    }

    private void ChangeSizeOnScroll()
    {
        if (Input.mouseScrollDelta.y > 0 && _currentCameraSize >= 5) 
            _camera.orthographicSize -= CameraSizeChange;
        else if (Input.mouseScrollDelta.y < 0 && _currentCameraSize < _defaultCameraSize) 
            _camera.orthographicSize += CameraSizeChange;
    }

    private void FocusPlayer()
    {
        _cameraPosition = transform.position;

        var minX = MapStartingCoordinate + _currentCameraSize * AspectRatio;
        var maxX = MapStartingCoordinate + mapWidth - _currentCameraSize * AspectRatio;

        if (_playerPosition.position.x < minX)
            _cameraPosition.x = minX;
        else if (_playerPosition.position.x > maxX)
            _cameraPosition.x = maxX;
        else
            _cameraPosition.x = _playerPosition.position.x;

        var minY = MapStartingCoordinate + _currentCameraSize;
        var maxY = MapStartingCoordinate + mapHeight - _currentCameraSize;

        if (_playerPosition.position.y < minY)
            _cameraPosition.y = minY;
        else if (_playerPosition.position.y > maxY)
            _cameraPosition.y = maxY;
        else
            _cameraPosition.y = _playerPosition.position.y;
        
        transform.SetPositionAndRotation(_cameraPosition, new Quaternion());
    }

    private void RenderBackground()
    {
        Vector3 position = new Vector3((mapWidth / 2) + MapStartingCoordinate, (mapHeight / 2) + MapStartingCoordinate, 0);

        GameObject background = Resources.Load<GameObject>("Background");
        background = Instantiate(background, position, Quaternion.identity);

        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        Vector3 backgroundSize = backgroundSpriteRenderer.bounds.size;
        
        background.transform.localScale = new Vector2(mapWidth / backgroundSize.x, mapHeight / backgroundSize.y);
    }
}
