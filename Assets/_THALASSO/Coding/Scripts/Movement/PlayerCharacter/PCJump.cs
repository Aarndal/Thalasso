using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PCJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [Header("Jump Variables")]
    [Tooltip("Defines the initial velocity applied to the PC at take-off in m/s.")]
    [SerializeField, Min(1f)]
    private float _takeoffVelocity = 10f;

    [Header("Gravity Variables")]
    [Tooltip("Will be added on top of Unity's gravity when PC is falling.")]
    [SerializeField, Min(0.0f)]
    private float _gravityIncrease = 10.0f;

    private Rigidbody _rigidbody = default;
    private bool _isGrounded = true;

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
        _input.JumpIsTriggered += OnJumpIsTriggered;
    }

    private void FixedUpdate()
    {
        SetPlayerGravity();
    }

    private void OnDisable()
    {
        _input.JumpIsTriggered -= OnJumpIsTriggered;
        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
    }
    #endregion

    private void OnJumpIsTriggered(bool isJumpTriggered)
    {
        if (isJumpTriggered && _isGrounded)
            _rigidbody.AddForce(Vector3.up * _rigidbody.mass * _takeoffVelocity, ForceMode.Impulse);
    }

    private void OnGroundedStateChanged(object[] args) => _isGrounded = (bool)args[0];

    private void SetPlayerGravity()
    {
        if (!_isGrounded && _rigidbody.linearVelocity.y <= 0)
            _rigidbody.AddForce(0.0f, -_gravityIncrease, 0.0f, ForceMode.Acceleration);
    }
}
