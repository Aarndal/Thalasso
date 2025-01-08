using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerInputReader _input;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField]
    private Transform _cameraRootTransform;

    [Header("Look Variables")]
    [Tooltip("Current tilt of the camera around the local x axis of the set camera root.")]
    [SerializeField]
    private float _cameraTilt = 0.0f;
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
    private float _rotationSmoothFactor = 0.35f;
    [Tooltip("How fast the player character can rotate left and right.")]
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float _horizontalSensitivity = 1.0f;
    [Tooltip("How fast the player character can look up and down.")]
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float _verticalSensitivity = 1.0f;

    private Vector3 _lookDirection = Vector3.zero;
    private Vector3 _previousLookDirection = Vector3.zero;
    private float _yaw = 0.0f;
    private float _rotationVelocity;

    [SerializeField]
    private readonly float ROTATION_TRESHOLD = 0.2f;

    public Transform CameraRoot { get => _cameraRootTransform; }

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
    }

    private void LateUpdate()
    {
        //RotatePlayer();
        //TiltCamera();
        CameraRotation();
    }

    private void OnDisable()
    {
        _input.LookInputHasChanged -= OnLookInputHasChanged;
    }
    #endregion

    private void OnLookInputHasChanged(Vector2 lookInput)
    {
        _previousLookDirection = _lookDirection;

        _lookDirection = new(
        x: (!_input.LookXInputIsInverted ? lookInput.x : -lookInput.x),
        y: (!_input.LookYInputIsInverted ? lookInput.y : -lookInput.y),
        z: 0.0f);
    }

    private void RotatePlayer()
    {
        if (_lookDirection == _previousLookDirection)
            return;

        //float deltaTime = IsCurrentDeviceMouse ? 1.0f : Time.fixedDeltaTime;
        float deltaTime = 1.0f;

        _lookDirection = Vector3.Lerp(_previousLookDirection, _lookDirection * deltaTime, _rotationSmoothFactor);

        _yaw += _lookDirection.x * _horizontalSensitivity;

        this.transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

        //if (_lookDirection.x >= ROTATION_TRESHOLD)
        //{
        //    _transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
        //        _transform.eulerAngles.y,
        //        targetRotation,
        //        ref _turnSmoothSpeed,
        //        _originSO.turnSmoothTime);
        //}
    }

    private void TiltCamera()
    {
        Vector3 currentCameraRotation = _cameraRootTransform.rotation.eulerAngles;
        _cameraTilt += _lookDirection.y * _verticalSensitivity;
        
        _cameraTilt = Mathf.Clamp(_cameraTilt, _bottomClampAngle, _topClampAngle);
        
        _cameraRootTransform.rotation = Quaternion.Euler(_cameraTilt, currentCameraRotation.y, currentCameraRotation.z);
    }

    private void CameraRotation()
    {
        if (_lookDirection.sqrMagnitude >= ROTATION_TRESHOLD)
        {
            ////Don't multiply mouse input by Time.deltaTime
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            float deltaTimeMultiplier = 1.0f;

            _cameraTilt += _lookDirection.y * _verticalSensitivity * deltaTimeMultiplier;
            _rotationVelocity = _lookDirection.x * _horizontalSensitivity * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cameraTilt = ClampAngle(_cameraTilt, _bottomClampAngle, _topClampAngle);

            // Update Cinemachine camera target pitch
            _cameraRootTransform.localRotation = Quaternion.Euler(_cameraTilt, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
