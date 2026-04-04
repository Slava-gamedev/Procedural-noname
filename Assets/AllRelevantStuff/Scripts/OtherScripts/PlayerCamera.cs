using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _playerCharacter;
    [SerializeField] private float _followSpeed;
    [SerializeField] private Transform _topBorder;
    [SerializeField] private Transform _bottomBorder;
    [SerializeField] private Transform _leftBorder;
    [SerializeField] private Transform _rightBorder;

    private Camera _camera;
    private float _halfWidth;
    private float _halfHeight;

    private float _minX, _maxX, _minY, _maxY;
    

    private void Start()
    {
        _camera = Camera.main;
        _halfHeight = _camera.orthographicSize;
        _halfWidth = _halfHeight * _camera.aspect;

        _minX = _leftBorder.position.x + _halfWidth;
        _maxX = _rightBorder.position.x - _halfWidth;
        _minY = _bottomBorder.position.y + _halfHeight;
        _maxY = _topBorder.position.y - _halfHeight;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = _playerCharacter.position;
        targetPosition.z = -10f;

        targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, _minY, _maxY);

        transform.position = targetPosition;// Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.fixedDeltaTime);
    }
}
