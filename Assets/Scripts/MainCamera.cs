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

    private Camera _camera;

    private bool _deathScreenVisible;

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    public GameObject deathScreenTintPrefab;
    private SpriteRenderer _deathScreenTintSpriteRenderer;

    void Start()
    {
        _playerPosition = GameObject.FindWithTag("Player").transform;
        _camera = GetComponent<Camera>();
        
        // The orthographicSize is half the size of the vertical viewing volume. -> https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html
        // At the Start, the orthographic size of camera is the same, as the half of map height
        _defaultCameraSize = _camera.orthographicSize;
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
    }

    private void InstantiateRedTint()
    {
        Vector3 position = new Vector3((MapStartingCoordinate + (2 * _defaultCameraSize) * AspectRatio) / 2f,
            (MapStartingCoordinate + 2 * _defaultCameraSize) / 2f, 0);
                
        GameObject deathScreenTint = Instantiate(deathScreenTintPrefab, position, Quaternion.identity);
        deathScreenTint.transform.localScale = new Vector2(160, 160);

        _deathScreenTintSpriteRenderer = deathScreenTint.GetComponent<SpriteRenderer>();

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
        if (Input.mouseScrollDelta.y > 0 && _camera.orthographicSize >= 5) 
            _camera.orthographicSize -= CameraSizeChange;
        else if (Input.mouseScrollDelta.y < 0 && _camera.orthographicSize < _defaultCameraSize) 
            _camera.orthographicSize += CameraSizeChange;
    }

    private void FocusPlayer()
    {
        _cameraPosition = transform.position;

        var currentCameraSize = _camera.orthographicSize;
        
        _minX = MapStartingCoordinate + currentCameraSize * AspectRatio; 
        _maxX = MapStartingCoordinate + (2 * _defaultCameraSize - currentCameraSize) * AspectRatio;
        
        if (_playerPosition.position.x < _minX)
            _cameraPosition.x = _minX;
        else if (_playerPosition.position.x > _maxX)
            _cameraPosition.x = _maxX;
        else
            _cameraPosition.x = _playerPosition.position.x;

        _minY = MapStartingCoordinate + currentCameraSize;
        _maxY = MapStartingCoordinate + 2 * _defaultCameraSize - currentCameraSize;
        
        if (_playerPosition.position.y < _minY)
            _cameraPosition.y = _minY;
        else if (_playerPosition.position.y > _maxY)
            _cameraPosition.y = _maxY;
        else
            _cameraPosition.y = _playerPosition.position.y;
        
        transform.SetPositionAndRotation(_cameraPosition, new Quaternion());
    }
}
