using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform _playerTransform;
    private Vector3 _cameraPosition;

    private float _defaultCameraSize;
    private float _currentCameraSize;
    private float _aspectRatio;

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    private Camera _camera;

    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        GameObject player = GameObject.FindWithTag(Constants.PlayerTag);
        _playerTransform = player.transform;
        
        _camera = GetComponent<Camera>();
        _aspectRatio = (float)Screen.width / Screen.height;
        
        // The orthographicSize is half the size of the vertical viewing volume. -> https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html
        // At the Start, the orthographic size of camera is the same, as the half of map height
        if (_camera.orthographicSize > Constants.MaxCameraSize) _camera.orthographicSize = Constants.MaxCameraSize;
        _defaultCameraSize = _currentCameraSize = _camera.orthographicSize;
        
        RenderBackground();
    }

    void LateUpdate()
    {
        if (!_playerTransform) return;
        CalculateCameraSizeAndPosition();
    }

    private void CalculateCameraSizeAndPosition()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float newSize = _camera.orthographicSize - (Input.mouseScrollDelta.y * Constants.CameraSizeChange);
            _camera.orthographicSize = Mathf.Clamp(newSize, 5f, _defaultCameraSize);
            FocusPlayer(true);
        } else FocusPlayer(false);
    }

    private void FocusPlayer(bool afterZoom)
    {
        CalculateCameraLimits();
        
        var current = _cameraPosition = transform.position;
        Vector3 playerPosition = _playerTransform.position;

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
        _maxX = Constants.MapStartingCoordinate + GlobalVariables.mapWidth - _currentCameraSize * _aspectRatio;

        _minY = Constants.MapStartingCoordinate + _currentCameraSize;
        _maxY = Constants.MapStartingCoordinate + GlobalVariables.mapHeight - _currentCameraSize;

        if (GlobalVariables.createLevelMode)
        {
            _maxY += CreateLevelPanelSize();
            _minY -= CreateLevelPanelSize();
        }
    }

    private void RenderBackground()
    {
        GameObject background = Resources.Load<GameObject>("ExtendableBackground");
        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        Vector3 backgroundSize = backgroundSpriteRenderer.bounds.size;

        Vector3 position = new Vector3(
            (backgroundSize.x / 2) + Constants.MapStartingCoordinate,
            (backgroundSize.y / 2) + Constants.MapStartingCoordinate,
            0
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
        
        background.transform.localScale = new Vector2(GlobalVariables.mapWidth / backgroundSize.x, GlobalVariables.mapHeight / backgroundSize.y);
    }

    private float CreateLevelPanelSize()
    {
        return _currentCameraSize * 2 * Constants.CreateLevelPanelScreenPercentage;
    }
}
