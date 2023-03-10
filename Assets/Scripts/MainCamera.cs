using UnityEngine;
using Color = UnityEngine.Color;

public class MainCamera : MonoBehaviour
{
    private Transform _playerPosition;
    private Vector3 _cameraPosition;

    private float _defaultCameraSize;
    private float _currentCameraSize;
    private float _aspectRatio;

    private Camera _camera;

    private bool _deathScreenVisible;

    public float mapHeight;
    public float mapWidth;

    private SpriteRenderer _deathScreenTintSpriteRenderer;

    void Start()
    {
        _playerPosition = GameObject.FindWithTag(Constants.PlayerTag).transform;
        _camera = GetComponent<Camera>();
        _aspectRatio = (float)Screen.width / Screen.height;
        
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
            UpdateRedTintOpacity(_deathScreenTintSpriteRenderer.color.a + Time.deltaTime / Constants.AppearingTime);
            return;
        }
        
        ChangeSizeOnScroll();
        FocusPlayer();

        _currentCameraSize = _camera.orthographicSize;
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

    private void ChangeSizeOnScroll()
    {
        if (Input.mouseScrollDelta.y > 0 && _currentCameraSize >= 5) 
            _camera.orthographicSize -= Constants.CameraSizeChange;
        else if (Input.mouseScrollDelta.y < 0 && _currentCameraSize < _defaultCameraSize) 
            _camera.orthographicSize += Constants.CameraSizeChange;
    }

    private void FocusPlayer()
    {
        _cameraPosition = transform.position;

        var minX = Constants.MapStartingCoordinate + _currentCameraSize * _aspectRatio;
        var maxX = Constants.MapStartingCoordinate + mapWidth - _currentCameraSize * _aspectRatio;

        if (_playerPosition.position.x < minX)
            _cameraPosition.x = minX;
        else if (_playerPosition.position.x > maxX)
            _cameraPosition.x = maxX;
        else
            _cameraPosition.x = _playerPosition.position.x;

        var minY = Constants.MapStartingCoordinate + _currentCameraSize;
        var maxY = Constants.MapStartingCoordinate + mapHeight - _currentCameraSize;

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
}
