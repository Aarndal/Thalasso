using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PCJump : MonoBehaviour
{
    [Tooltip("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [SerializeField, Min(1f)]
    private float _defaultJumpForce = 10f;

    //[Header("Gravity Variables")]
    //[Tooltip("Will be added on top of Unity's gravity.")]
    //[SerializeField][Range(0.0f, 100.0f)] private float gravityIncrease = 10.0f;

    private Rigidbody _rigidbody = default;
    private bool _isGrounded = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
        _input.JumpIsTriggered += OnJumpIsTriggered;
    }

    private void OnDisable()
    {
        _input.JumpIsTriggered -= OnJumpIsTriggered;
        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
    }

    private void OnJumpIsTriggered(bool isJumpTriggered)
    {
        if (isJumpTriggered && _isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * _rigidbody.mass * _defaultJumpForce, ForceMode.Impulse);
        }
    }

    private void OnGroundedStateChanged(object[] args)
    {
        _isGrounded = (bool)args[0];
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
