using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour, IAmMovable
{
    [Header("References")]
    [SerializeField] private PlayerInputReader _input;
    
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

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.MoveInputHasChanged += OnMoveInputHasChanged;
        _input.SprintIsTriggered += OnSprintIsTriggered;
    }

    private void Start()
    {
        _rigidbody.freezeRotation = true;
        _speedFactor = _walkingSpeed;
    }

    private void FixedUpdate()
    {
        Move();
        
        //SetPlayerGravity();
        //Jump();
    }

    private void OnDisable()
    {
        _input.MoveInputHasChanged -= OnMoveInputHasChanged;
        _input.SprintIsTriggered -= OnSprintIsTriggered;
    }
    #endregion

    #region Delegate Methods
    private void OnMoveInputHasChanged(Vector2 moveInput)
    {
        //if (!playerIsGrounded.Value)
        //    PlayerSpeed *= inAirMovementMultiplier;

        _moveDirection = new(
            x: moveInput.x,
            y: 0.0f,
            z: moveInput.y);

        Debug.Log("Move Input: " + _moveDirection);
    }

    private void OnSprintIsTriggered(bool sprinting)
    {
        _speedFactor = (sprinting == false ? _walkingSpeed : _sprintSpeed);
    }
    #endregion

    #region Interface Implementation
    public void Move()
    {
        //if (!playerIsGrounded.Value)
        //    PlayerSpeed *= inAirMovementMultiplier;
        _rigidbody.AddRelativeForce(force: _rigidbody.mass * _speedFactor * _moveDirection / Time.fixedDeltaTime, mode: ForceMode.Force);
    }
    #endregion

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
