using UnityEngine;
using Color = UnityEngine.Color;

public class MainCamera : MonoBehaviour
{
    private Transform _playerTransform;
    private SpriteRenderer _playerSpriteRenderer;
    private Vector3 _cameraPosition;

    private float _defaultCameraSize;
    private float _currentCameraSize;
    private float _aspectRatio;

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    private Camera _camera;

    private bool _deathScreenVisible;

    public float mapHeight;
    public float mapWidth;

    private SpriteRenderer _deathScreenTintSpriteRenderer;

    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        GameObject player = GameObject.FindWithTag(Constants.PlayerTag);
        _playerTransform = player.transform;
        _playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        
        _camera = GetComponent<Camera>();
        _aspectRatio = (float)Screen.width / Screen.height;
        
        // The orthographicSize is half the size of the vertical viewing volume. -> https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html
        // At the Start, the orthographic size of camera is the same, as the half of map height
        _defaultCameraSize = _currentCameraSize = _camera.orthographicSize;
        
        RenderBackground();
    }

    void LateUpdate()
    {
        if (!_playerTransform)
        {
            if (!_deathScreenVisible) InstantiateRedTint();
            UpdateRedTintOpacity(_deathScreenTintSpriteRenderer.color.a + Time.deltaTime / Constants.AppearingTime);
            return;
        }
        
        CalculateCameraSizeAndPosition();
    }

    private void InstantiateRedTint()
    {
        Vector3 position = new Vector3(_cameraPosition.x, _cameraPosition.y, 0);

        GameObject deathScreenTint = Resources.Load<GameObject>("DeathScreenTint");
        deathScreenTint = Instantiate(deathScreenTint, position, Quaternion.identity);

        _deathScreenTintSpriteRenderer = deathScreenTint.GetComponent<SpriteRenderer>();
        Vector3 tintSize = _deathScreenTintSpriteRenderer.bounds.size;
        
        deathScreenTint.transform.localScale = new Vector2(2 * _currentCameraSize * _aspectRatio / tintSize.x, 2 * _currentCameraSize / tintSize.y);

        UpdateRedTintOpacity(0);

        _deathScreenVisible = true;
    }

    private void UpdateRedTintOpacity(float opacity)
    {
        if (opacity <= Constants.MaxRedTintOpacity)
        {
            Color deathScreenColor = _deathScreenTintSpriteRenderer.color;
            deathScreenColor.a = opacity;
            _deathScreenTintSpriteRenderer.color = deathScreenColor;
        }
    }

    private void CalculateCameraSizeAndPosition()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float newSize = _camera.orthographicSize - (Input.mouseScrollDelta.y * Constants.CameraSizeChange);
            _camera.orthographicSize = Mathf.Clamp(newSize, 5f, _defaultCameraSize);
            FocusPlayer(Input.mouseScrollDelta.y < 0);
        } else FocusPlayer(false);
    }

    private void FocusPlayer(bool afterZoom)
    {
        CalculateCameraLimits();
        
        var current = _cameraPosition = transform.position;
        Vector3 cameraOffsetFromPlayer = new Vector3(_playerSpriteRenderer.flipX ? -5f : 5f, 0, 0);
        Vector3 playerPosition = _playerTransform.position + cameraOffsetFromPlayer;

        _cameraPosition.x = Mathf.Clamp(playerPosition.x, _minX, _maxX);
        _cameraPosition.y = Mathf.Clamp(playerPosition.y, _minY, _maxY);

        transform.position = afterZoom 
            ? _cameraPosition
            : Vector3.SmoothDamp(current, _cameraPosition, ref _velocity, 0.5f);
    }

    private void CalculateCameraLimits()
    {
        _currentCameraSize = _camera.orthographicSize;
        
        _minX = Constants.MapStartingCoordinate + _currentCameraSize * _aspectRatio;
        _maxX = Constants.MapStartingCoordinate + mapWidth - _currentCameraSize * _aspectRatio;
        
        _minY = Constants.MapStartingCoordinate + _currentCameraSize;
        _maxY = Constants.MapStartingCoordinate + mapHeight - _currentCameraSize;

        if (GlobalVariables.createLevelMode) _minY -= CreateLevelPanelSize();
    }

    private void RenderBackground()
    {
        Vector3 position = new Vector3(
            (mapWidth / 2) + Constants.MapStartingCoordinate,
            (mapHeight / 2) + Constants.MapStartingCoordinate,
            0
            );

        GameObject background = Resources.Load<GameObject>("Background");
        background = Instantiate(background, position, Quaternion.identity);

        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        Vector3 backgroundSize = backgroundSpriteRenderer.bounds.size;
        
        background.transform.localScale = new Vector2(mapWidth / backgroundSize.x, mapHeight / backgroundSize.y);
    }

    private float CreateLevelPanelSize()
    {
        return _currentCameraSize * 2 * Constants.CreateLevelPanelScreenPercentage;
    }
}
