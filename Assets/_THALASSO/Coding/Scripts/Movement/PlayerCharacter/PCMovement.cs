using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PCMovement : MonoBehaviour, IAmMovable
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;
    [SerializeField]
    private SlopeChecker _slopeChecker = default;

    [Header("Movement Variables")]
    [Tooltip("Movement speed of the player character in m/s.")]
    [SerializeField, Min(0.0f)]
    private float _walkingSpeed = 1.0f;
    [Tooltip("Sprinting speed of the player character in m/s.")]
    [SerializeField, Min(0.0f)]
    private float _sprintSpeed = 3.0f;

    [Tooltip("Multiplier for the movement speed of the player character while in the air.")]
    [SerializeField, Min(0.0f)]
    private float _inAirSpeed = 0.5f;

    private Rigidbody _rigidbody = default;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private float _speedFactor = 0.0f;
    private bool _isSprinting = false;
    private bool _isGrounded = true;
    private bool _slopeDetected = false;

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //_previousMoveDirection = _moveDirection;
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
        _input.MoveInputHasChanged += OnMoveInputHasChanged;
        _input.SprintIsTriggered += OnSprintIsTriggered;
        _slopeChecker.SlopeDetected += OnSlopeDetected;
    }


    private void Start()
    {
        _rigidbody.freezeRotation = true;
        _speedFactor = _walkingSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
        _input.MoveInputHasChanged -= OnMoveInputHasChanged;
        _input.SprintIsTriggered -= OnSprintIsTriggered;
        _slopeChecker.SlopeDetected -= OnSlopeDetected;
    }
    #endregion

    private void OnGroundedStateChanged(object[] args)
    {
        _isGrounded = (bool)args[0];
    }

    private void OnMoveInputHasChanged(Vector2 moveInput)
    {
        //_previousMoveDirection = _moveDirection;

        _moveDirection = new(
            x: moveInput.x,
            y: 0.0f,
            z: moveInput.y);
    }

    private void OnSlopeDetected(bool slopeDetected)
    {
        _slopeDetected = slopeDetected;
    }

    private void OnSprintIsTriggered(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }

    public void Move()
    {
        if (!_isGrounded)
            _speedFactor = _inAirSpeed;

        if(_isGrounded)
            _speedFactor = _isSprinting == false ? _walkingSpeed : _sprintSpeed;

        //float playerAcceleration = PlayerSpeed * moveDirection / Time.fixedDeltaTime;

        //playerRigidbody.AddForce(playerRigidbody.mass * playerAcceleration * Vector2.right, ForceMode2D.Force);

        //if (input.SprintIsTriggered && Mathf.Abs(playerRigidbody.velocity.x) > sprintSpeed)
        //    playerRigidbody.velocity = new(sprintSpeed * Mathf.Sign(playerRigidbody.velocity.x), playerRigidbody.velocity.y);
        //else if (!input.SprintIsTriggered && Mathf.Abs(playerRigidbody.velocity.x) > walkingSpeed)
        //    playerRigidbody.velocity = new(walkingSpeed * Mathf.Sign(playerRigidbody.velocity.x), playerRigidbody.velocity.y);

        _velocity = _slopeDetected ? _slopeChecker.Velocity : _speedFactor * _moveDirection;

        _rigidbody.AddRelativeForce(force: _rigidbody.mass * _velocity / Time.fixedDeltaTime, mode: ForceMode.Force);
    }
}
