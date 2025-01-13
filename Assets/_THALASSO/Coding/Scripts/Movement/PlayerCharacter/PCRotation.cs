using UnityEngine;

public class PCRotation : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField]
    private Transform _cameraRoot;

    [Header("Look Variables")]
    [SerializeField]
    [Range(1.0f, 2.0f)]
    private float _rotationThreshold = 1.0f;
    [Tooltip("How far in degrees can you move the camera up.")]
    [SerializeField]
    private float _topClampAngle = 80.0f;
    [Tooltip("How far in degrees can you move the camera down.")]
    [SerializeField]
    private float _bottomClampAngle = -80.0f;

    [Space(10)]
    [Tooltip("How smooth the player character rotates towards the look direction.")]
    [SerializeField]
    [Range(0.1f, 0.9f)]
    private float _yawSmoothFactor =  0.5f;
    [SerializeField]
    [Range(0.1f, 0.9f)]
    private float _pitchSmoothFactor = 0.5f;
    [Tooltip("How fast the player character can rotate left and right.")]
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float _horizontalSensitivity = 1.0f;
    [Tooltip("How fast the player character can look up and down.")]
    [SerializeField]
    [Range(0.01f, 1.0f)]
    private float _verticalSensitivity = 0.1f;

    private Vector3 _lookDirection = Vector3.zero;
    private Vector3 _previousLookDirection = Vector3.zero;
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;

    public Transform CameraRoot { get => _cameraRoot; }

    //private bool IsCurrentDeviceMouse
    //{
    //    get { return playerInput.currentControlScheme == "Keyboard and Mouse"; }
    //}

    #region Unity MonoBehaviour Methods
    private void OnEnable()
    {
        _input.LookInputHasChanged += OnLookInputHasChanged;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _pitch = _cameraRoot.localEulerAngles.x;
        _yaw = this.transform.localEulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnDisable()
    {
        _input.LookInputHasChanged -= OnLookInputHasChanged;
    }
    #endregion

    private void OnLookInputHasChanged(Vector2 lookInput, bool isCurrentDeviceMouse)
    {
        _previousLookDirection = _lookDirection;

        _lookDirection = new(
        x: (!_input.IsXLookInputInverted ? lookInput.x : -lookInput.x),
        y: (_input.IsYLookInputInverted ? lookInput.y : -lookInput.y),
        z: 0.0f);
    }

    private void CameraRotation()
    {
        if (_lookDirection.sqrMagnitude >= _rotationThreshold)
        {
            ////Don't multiply mouse input by Time.deltaTime
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            float deltaTimeMultiplier = 1.0f;

            _pitch += Mathf.LerpAngle(_previousLookDirection.y, _lookDirection.y, _pitchSmoothFactor) * _verticalSensitivity * deltaTimeMultiplier;
            _yaw += Mathf.LerpAngle(_previousLookDirection.x, _lookDirection.x, _yawSmoothFactor) * _horizontalSensitivity * deltaTimeMultiplier;

            //_pitch = ClampAngle(_pitch, _bottomClampAngle, _topClampAngle);
            _pitch = Mathf.Clamp(_pitch, _bottomClampAngle, _topClampAngle);

            _cameraRoot.localRotation = Quaternion.Euler(-_pitch, 0.0f, 0.0f);

            this.transform.localRotation = Quaternion.Euler(0.0f, _yaw, 0.0f);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
