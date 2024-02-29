using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Set Up")]
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Transform _cameraTransform;

    [Header("Camera settings")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _movementTime;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private Vector3 _zoomAmount;
    [SerializeField]
    private Vector3 _minZoomAmount;
    [SerializeField]
    private Vector3 _maxZoomAmount;

    private Vector3 _nextPosition;
    private Vector3 _nextZoom;
    private Quaternion _nextRotation;

    private Plane _upPlane;
    private Vector3 dragStartPoint;
    private Vector3 dragCurrentPoint;

    private Unit _followUnit;
    private int _followIndex = -1;

    private float _keyLockTime = .4f;
    private float _sinceLastLock = 0f;


    // Start is called before the first frame update
    void Start()
    {
        _nextPosition = transform.position;
        _nextRotation = transform.rotation;
        _nextZoom = _cameraTransform.localPosition;
        _upPlane = new Plane(Vector3.up, Vector3.zero);
        _followUnit = UnitManager.Instance.GetUnitAtIndex(0, GameManager.Instance.PlayerTeam);
    }

    // Update is called once per frame
    void Update()
    {
        if (_sinceLastLock < _keyLockTime + 1f) _sinceLastLock += Time.deltaTime;

        HandleCameraKeyboardInput();
    }

    private void HandleCameraKeyboardInput()
    {
        if (_followUnit != null) _nextPosition = _followUnit.gameObject.transform.position;

        if (_groundLayer == 0) Debug.LogError("No ground layer assigned to camera script");
        if (_movementSpeed == 0) Debug.LogWarning("camera movement speed set to 0");
        if (_movementTime == 0) Debug.LogWarning("camera movement time set to 0");
        if (_rotationSpeed == 0) Debug.LogWarning("camera rotation speed set to 0");

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _nextPosition += (transform.forward * _movementSpeed);
            GameManager.Instance.CameraTravelDistance += (transform.forward * _movementSpeed).magnitude;
            _followUnit = null;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _nextPosition += (transform.forward * -_movementSpeed);
            GameManager.Instance.CameraTravelDistance += (transform.forward * _movementSpeed).magnitude;
            _followUnit = null;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _nextPosition += (transform.right * -_movementSpeed);
            GameManager.Instance.CameraTravelDistance += (transform.right * _movementSpeed).magnitude;
            _followUnit = null;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _nextPosition += (transform.right * _movementSpeed);
            GameManager.Instance.CameraTravelDistance += (transform.right * _movementSpeed).magnitude;
            _followUnit = null;
        }

        if (Input.GetKey(KeyCode.Tab) && _sinceLastLock > _keyLockTime)
        {
            _sinceLastLock = 0;

            ETeam playerTeam = GameManager.Instance.PlayerTeam;
            if (_followIndex + 1 >= UnitManager.Instance.GetTeamSize(playerTeam)) _followIndex = 0;
            else _followIndex++;

            _followUnit = UnitManager.Instance.GetUnitAtIndex(_followIndex, playerTeam);
        }

        if (Input.GetKey(KeyCode.Q)) _nextRotation *= Quaternion.Euler(Vector3.up * _rotationSpeed);
        if (Input.GetKey(KeyCode.E)) _nextRotation *= Quaternion.Euler(Vector3.up * -_rotationSpeed);

        if (Input.GetKey(KeyCode.R))
        {
            if (_nextZoom.y + _zoomAmount.y > _minZoomAmount.y && _nextZoom.z + _zoomAmount.z < _minZoomAmount.z) _nextZoom += _zoomAmount;
        }

        if (Input.GetKey(KeyCode.F))
        {
            
            if (_nextZoom.y - _zoomAmount.y < _maxZoomAmount.y && _nextZoom.z - _zoomAmount.z > _maxZoomAmount.z) _nextZoom -= _zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, _nextPosition, Time.deltaTime * _movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _nextRotation, Time.deltaTime * _movementTime);
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _nextZoom, Time.deltaTime * _movementTime);
    }
}
