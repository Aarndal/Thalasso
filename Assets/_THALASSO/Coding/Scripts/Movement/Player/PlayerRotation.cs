using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerInputReader _input = default;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField]
    private Transform _cameraRoot;

    [Header("Look Variables")]
    [SerializeField]
    private const float ROTATION_TRESHOLD = 0.2f;
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
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;
    private float _rotationVelocity;

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

    private void OnLookInputHasChanged(Vector2 lookInput)
    {

        _previousLookDirection = _lookDirection;

        //if (lookInput.sqrMagnitude <= ROTATION_TRESHOLD)
        //{
        //    _lookDirection = Vector3.zero;
        //    return;
        //}

        _lookDirection = new(
        x: (!_input.LookXInputIsInverted ? lookInput.x : -lookInput.x),
        y: (!_input.LookYInputIsInverted ? lookInput.y : -lookInput.y),
        z: 0.0f);
    }

    private void CameraRotation()
    {
        if (_lookDirection.sqrMagnitude >= ROTATION_TRESHOLD)
        {
            ////Don't multiply mouse input by Time.deltaTime
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            float deltaTimeMultiplier = 1.0f;

            _pitch += _lookDirection.y * _verticalSensitivity * deltaTimeMultiplier;
            _yaw += _lookDirection.x * _horizontalSensitivity * deltaTimeMultiplier;

            // clamp our pitch rotation
            _pitch = ClampAngle(_pitch, _bottomClampAngle, _topClampAngle);

            // Update camera target pitch
            _cameraRoot.localRotation = Quaternion.Euler(_pitch, 0.0f, 0.0f);

            // rotate the player left and right
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
