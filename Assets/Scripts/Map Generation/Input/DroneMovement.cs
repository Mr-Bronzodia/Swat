using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DroneMovement : MonoBehaviour
{
    private const int _updateRate = 5;

    private const int _cameraMaxX = 20;
    private const int _cameraMaxY = 20;

    [SerializeField]
    private Vector3[] _droneMovementPoints;
    [SerializeField]
    private GameObject _droneCamera;

    private int _currentPointIndex = 0;
    private int _sinceLastUpdate = 0;
    private Quaternion _desiredRotation;
    private Vector3 _lastMousePosition;

    private void Awake()
    {
        Assert.AreNotEqual(_droneMovementPoints.Length, 0, "No drone movement points assigned in inspector");
        _desiredRotation = transform.rotation;
        _lastMousePosition = Input.mousePosition;
    }

    private void HandleMouseInput()
    {
        Vector3 mouseDelta = Input.mousePosition - _lastMousePosition;
        Vector3 nextRotation = Vector3.zero;

        if (_desiredRotation.eulerAngles.x < _cameraMaxX && _desiredRotation.eulerAngles.x >= -_cameraMaxX)
        {
            nextRotation.x += _droneCamera.transform.localRotation.eulerAngles.x - mouseDelta.y;
        }

        if (_desiredRotation.eulerAngles.y < _cameraMaxY && _desiredRotation.eulerAngles.y >= -_cameraMaxY)
        {
            nextRotation.y += _droneCamera.transform.localRotation.eulerAngles.y + mouseDelta.x;
        }

        //_desiredRotation = Quaternion.Euler(_droneCamera.transform.localRotation.eulerAngles.x - mouseDelta.y, _droneCamera.transform.localRotation.eulerAngles.y + mouseDelta.x, 0f);

        _desiredRotation.eulerAngles = nextRotation;

        _lastMousePosition = Input.mousePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = _droneMovementPoints[_currentPointIndex];
    }

    // Update is called once per frame
    void Update()
    {

        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _droneMovementPoints[_currentPointIndex], );
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _droneMovementPoints[_currentPointIndex], 2f * Time.deltaTime);

        _sinceLastUpdate++;
        if (_sinceLastUpdate <= _updateRate) return;

        if (Vector3.Distance(gameObject.transform.position, _droneMovementPoints[_currentPointIndex]) > 3f) return;

        if (_currentPointIndex + 1 >= _droneMovementPoints.Length) _currentPointIndex = 0;
        else _currentPointIndex++;
    }

    private void LateUpdate()
    {
        HandleMouseInput();
        _droneCamera.transform.localRotation = Quaternion.Slerp(_droneCamera.transform.localRotation, _desiredRotation, .9f * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        foreach (var point in _droneMovementPoints)
        {
            Gizmos.DrawSphere(point, 1f);
        }

    }
}
