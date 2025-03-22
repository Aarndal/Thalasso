using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PCRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField]
    private Transform _cameraRoot;

    [Header("Look Variables")]
    [Tooltip("Defines the minimum threshold value for registering the look input for the rotation. Is compared with the squared magnitude of the look input vector.")]
    [SerializeField, Range(0.001f, 0.01f)]
    private float _rotationThreshold = 0.001f;
    [Tooltip("How far in degrees can you move the camera up.")]
    [SerializeField, Range(30.0f, 80.0f)]
    private float _topClampAngle = 80.0f;
    [Tooltip("How far in degrees can you move the camera down.")]
    [SerializeField, Range(-30.0f, -80.0f)]
    private float _bottomClampAngle = -80.0f;

    [Space(10)]
    
    [Tooltip("How fast the player character can rotate left and right.")]
    [SerializeField, Range(0.01f, 2.0f)]
    private float _horizontalSensitivity = 1.0f;
    [Tooltip("How fast the player character can look up and down.")]
    [SerializeField, Range(0.01f, 2.0f)]
    private float _verticalSensitivity = 1.0f;

    private Rigidbody _rigidbody;
    private Vector3 _lookDirection = Vector3.zero;
    private float _deltaTimeMultiplier = 1.0f; //? Obsolete because of Delta Time Scale processor in Input Action Asset?
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;

    public Transform CameraRoot { get => _cameraRoot; }

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.LookInputHasChanged += OnLookInputHasChanged;
    }

    private void Start()
    {
        _deltaTimeMultiplier = 1.0f;

        _pitch = _cameraRoot.localEulerAngles.x;
        _yaw = transform.localEulerAngles.y;
    }

    private void OnDisable()
    {
        _input.LookInputHasChanged -= OnLookInputHasChanged;
    }
    #endregion

    private void OnLookInputHasChanged(Vector2 lookInput, bool isCurrentDeviceMouse)
    {
        _deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime; //? Obsolete because of Delta Time Scale processor in Input Action Asset?
        
        _lookDirection = new(
        x: (!_input.IsXLookInputInverted ? lookInput.x : -lookInput.x),
        y: (_input.IsYLookInputInverted ? lookInput.y : -lookInput.y),
        z: 0.0f);
        
        Rotate();
    }

    private void Rotate()
    {
        if (_lookDirection.sqrMagnitude >= _rotationThreshold)
        {
            _pitch += _lookDirection.y * _verticalSensitivity * _deltaTimeMultiplier;
            _pitch = ClampAngle(_pitch, _bottomClampAngle, _topClampAngle);
            _cameraRoot.localRotation = Quaternion.Euler(-_pitch, 0.0f, 0.0f);

            _yaw += _lookDirection.x * _horizontalSensitivity * _deltaTimeMultiplier;
            _rigidbody.MoveRotation(Quaternion.Euler(0.0f, _yaw, 0.0f));
        }
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
