using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform _playerTransform;
    private Vector3 _cameraPosition;

    private float _defaultCameraSize;
    private float _currentCameraSize;
    private float _aspectRatio;

    private float _minX, _maxX, _minY, _maxY;

    private Camera _camera;

    [SerializeField] private float mapWidth, mapHeight;

    void Start()
    {
        if (mapWidth != 0 && mapHeight != 0)
        {
            GlobalVariables.mapWidth = mapWidth;
            GlobalVariables.mapHeight = mapHeight;
        }

        GameObject player = GameObject.FindWithTag(Constants.PlayerTag);
        _playerTransform = player.transform;
        
        _camera = GetComponent<Camera>();
        _aspectRatio = (float)Screen.width / Screen.height;
        
        // The orthographicSize is half the size of the vertical viewing volume. -> https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html
        // At the Start, the orthographic size of camera is the same, as the half of map height
        if (_camera.orthographicSize > Constants.MaxCameraSize) _camera.orthographicSize = Constants.MaxCameraSize;
        _defaultCameraSize = _currentCameraSize = _camera.orthographicSize;
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
            _currentCameraSize = _camera.orthographicSize = Mathf.Clamp(newSize, Constants.MinCameraSize, _defaultCameraSize);
        }
        
        FocusPlayer();
    }

    private void FocusPlayer()
    {
        CalculateCameraLimits();
        
        _cameraPosition = transform.position;
        Vector3 playerPosition = _playerTransform.position;

        _cameraPosition.x = Mathf.Clamp(playerPosition.x, _minX, _maxX);
        _cameraPosition.y = Mathf.Clamp(playerPosition.y, _minY, _maxY);

        transform.position = _cameraPosition;
    }

    private void CalculateCameraLimits()
    {
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

    private float CreateLevelPanelSize()
    {
        return _currentCameraSize * 2 * Constants.CreateLevelPanelScreenPercentage;
    }
}
