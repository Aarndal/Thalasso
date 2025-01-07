using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader _input;
    [Tooltip("Target that the camera is following and on which it is centered.")]
    [SerializeField] private Transform _playerCameraRoot;

    [Header("Look Variables")]
    [Tooltip("Current tilt of the camera around the local x axis of the set camera root.")]
    [SerializeField] private float _cameraTilt = 0.0f;
    [Tooltip("How far in degrees can you move the camera up.")]
    [SerializeField] private float _topClampAngle = 80.0f;
    [Tooltip("How far in degrees can you move the camera down.")]
    [SerializeField] private float _bottomClampAngle = -80.0f;

    [Space(10)]
    [Tooltip("How smooth the player character rotates towards the look direction.")]
    [SerializeField][Range(0.1f, 0.9f)] private float _rotationSmoothFactor = 0.35f;
    [Tooltip("How fast the player character can rotate left and right.")]
    [SerializeField][Range(0.1f, 10.0f)] private float _horizontalSensitivity = 1.0f;
    [Tooltip("How fast the player character can look up and down.")]
    [SerializeField][Range(0.1f, 10.0f)] private float _verticalSensitivity = 1.0f;

    [Header("Movement Variables")]
    [Tooltip("Movement speed of the player character in m/s.")]
    [SerializeField][Range(0.0f, 10.0f)] private float _walkingSpeed = 1.0f;
    [Tooltip("Sprinting speed of the player character in m/s.")]
    [SerializeField][Range(0.0f, 10.0f)] private float _sprintSpeed = 3.0f;
    //[Tooltip("Multiplier for the movement speed of the player character while in the air.")]
    //[SerializeField][Range(0.0f, 1.0f)] private float inAirMovementMultiplier = 0.5f;

    //[Header("Jump Variables")]
    //[Tooltip("The velocity with which the player character is jumping in m/s.")]
    //[SerializeField][Range(0.0f, 10.0f)] private float verticalVelocity = 2.0f;
    //[Tooltip("Time in seconds the player input is taken into account for the jump after leaving the ground.")]
    //[SerializeField][Range(0.0f, 0.2f)] private float timeToMaxJump = 0.15f;
    //[SerializeField] private float counterToMaxJump = 0.0f;

    //[Header("Gravity Variables")]
    //[Tooltip("Will be added on top of Unity's gravity.")]
    //[SerializeField][Range(0.0f, 100.0f)] private float gravityIncrease = 10.0f;

    private Rigidbody _rigidbody;
    private Vector3 _moveDirection = Vector3.zero;
    private float _speedFactor = 0.0f;
    private Vector3 playerLookDirection = Vector3.zero;
    private Vector3 previousPlayerLookDirection = Vector3.zero;
    private float playerYRotation = 0.0f;

    public Transform PlayerCameraRoot { get => _playerCameraRoot; }
    //private bool IsCurrentDeviceMouse
    //{
    //    get { return PlayerInputHandler.Instance.PlayerInput.currentControlScheme == "Keyboard and Mouse"; }
    //}

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.MoveInputHasChanged += OnMove;
        _input.SprintIsTriggered += OnSprint;
    }


    private void Start()
    {
        _rigidbody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _speedFactor = _walkingSpeed;
    }

    private void FixedUpdate()
    {
        //playerLookDirection = GetLookInput();
        //RotatePlayer();
        //TiltCamera();

        MovePlayer();
        
        //SetPlayerGravity();
        //Jump();
    }

    private void OnDisable()
    {
        _input.MoveInputHasChanged -= OnMove;
        _input.SprintIsTriggered -= OnSprint;
    }
    #endregion

    //private Vector3 GetLookInput()
    //{
    //    previousPlayerLookDirection = playerLookDirection;

    //    playerLookDirection = new(
    //    x: (!PlayerInputHandler.Instance.LookXInputIsInverted ? PlayerInputHandler.Instance.LookInput.x : -PlayerInputHandler.Instance.LookInput.x),
    //    y: (!PlayerInputHandler.Instance.LookYInputIsInverted ? PlayerInputHandler.Instance.LookInput.y : -PlayerInputHandler.Instance.LookInput.y),
    //    z: 0.0f);

    //    float deltaTime = IsCurrentDeviceMouse ? 1.0f : Time.fixedDeltaTime;

    //    return Vector3.Lerp(previousPlayerLookDirection, playerLookDirection * deltaTime, rotationSmoothFactor);
    //}

    //private void RotatePlayer()
    //{
    //    playerYRotation += playerLookDirection.x * horizontalSensitivity;

    //    playerRigidbody.rotation = Quaternion.Euler(0f, playerYRotation, 0f);
    //}

    //private void TiltCamera()
    //{
    //    Vector3 currentCameraRotation = playerCameraRoot.rotation.eulerAngles;
    //    cameraTilt += playerLookDirection.y * verticalSensitivity;

    //    cameraTilt = Mathf.Clamp(cameraTilt, bottomClampAngle, topClampAngle);

    //    playerCameraRoot.rotation = Quaternion.Euler(cameraTilt, currentCameraRotation.y, currentCameraRotation.z);
    //}

    private void OnMove(Vector2 moveInput)
    {
        //if (!playerIsGrounded.Value)
        //    PlayerSpeed *= inAirMovementMultiplier;

        _moveDirection = new(
            x: moveInput.x,
            y: 0.0f,
            z: moveInput.y);

        Debug.Log("Move Input: " + _moveDirection);
    }

    private void OnSprint(bool sprinting)
    {
        _speedFactor = (sprinting == false ? _walkingSpeed : _sprintSpeed);
    }

    private void MovePlayer()
    {

        //if (!playerIsGrounded.Value)
        //    PlayerSpeed *= inAirMovementMultiplier;

        _rigidbody.AddRelativeForce(force: _rigidbody.mass * _speedFactor * _moveDirection / Time.fixedDeltaTime, mode: ForceMode.Force);
    }

    //private void SetPlayerGravity()
    //{
    //    if (playerIsGrounded.Value)
    //        playerRigidbody.useGravity = false;
    //    else
    //    {
    //        playerRigidbody.useGravity = true;
    //        playerRigidbody.AddForce(0.0f, -gravityIncrease, 0.0f, ForceMode.Acceleration);
    //    }
    //}

    //private void Jump()
    //{
    //    SetMaxJumpTimer();
    //    if (counterToMaxJump > 0f && input.JumpIsTriggered)
    //        playerRigidbody.AddForce(force: playerRigidbody.mass * verticalVelocity * Vector3.up, mode: ForceMode.Impulse);
    //    else
    //        counterToMaxJump = 0f;
    //}

    //private void SetMaxJumpTimer()
    //{
    //    if (playerIsGrounded.Value)
    //        counterToMaxJump = timeToMaxJump;
    //    else
    //        counterToMaxJump -= Time.fixedDeltaTime;
    //}

}
