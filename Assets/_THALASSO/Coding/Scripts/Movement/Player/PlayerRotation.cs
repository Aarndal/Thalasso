using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerInputReader _input;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField]
    private Transform _playerCameraRoot;

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

    private Vector3 _playerLookDirection = Vector3.zero;
    private Vector3 _previousPlayerLookDirection = Vector3.zero;
    private float _playerYRotation = 0.0f;

    public Transform PlayerCameraRoot { get => _playerCameraRoot; }
    //private bool IsCurrentDeviceMouse
    //{
    //    get { return _input.currentControlScheme == "Keyboard and Mouse"; }
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

    private void FixedUpdate()
    {
        RotatePlayer();
        TiltCamera();
    }
    #endregion

    private void OnLookInputHasChanged(Vector2 lookInput)
    {
        _previousPlayerLookDirection = _playerLookDirection;

        _playerLookDirection = new(
        x: (!_input.LookXInputIsInverted ? lookInput.x : -lookInput.x),
        y: (!_input.LookYInputIsInverted ? lookInput.y : -lookInput.y),
        z: 0.0f);

        //float deltaTime = IsCurrentDeviceMouse ? 1.0f : Time.fixedDeltaTime;
        float deltaTime = Time.fixedDeltaTime;

        _playerLookDirection = Vector3.Lerp(_previousPlayerLookDirection, _playerLookDirection * deltaTime, _rotationSmoothFactor);
    }

    private void RotatePlayer()
    {
        _playerYRotation += _playerLookDirection.x * _horizontalSensitivity;

        //playerRigidbody.rotation = Quaternion.Euler(0f, _playerYRotation, 0f);
    }

    private void TiltCamera()
    {
        Vector3 currentCameraRotation = _playerCameraRoot.rotation.eulerAngles;
        _cameraTilt += _playerLookDirection.y * _verticalSensitivity;
        
        _cameraTilt = Mathf.Clamp(_cameraTilt, _bottomClampAngle, _topClampAngle);
        
        _playerCameraRoot.rotation = Quaternion.Euler(_cameraTilt, currentCameraRotation.y, currentCameraRotation.z);
    }
}
