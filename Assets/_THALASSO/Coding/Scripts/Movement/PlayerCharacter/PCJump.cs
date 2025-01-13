using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PCJump : MonoBehaviour
{
    [Tooltip("References")]
    [SerializeField]
    private SO_GameInputReader _gameInput;

    [SerializeField]
    private float _defaultJumpForce = 10f;

    private Rigidbody _rigidbody;
    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _gameInput.JumpIsTriggered += OnJumpIsTriggered;
        GlobalEventBus.Register(GlobalEvents.Player.IsGrounded, OnIsGrounded);
    }


    private void OnDisable()
    {
        _gameInput.JumpIsTriggered -= OnJumpIsTriggered;
        GlobalEventBus.Deregister(GlobalEvents.Player.IsGrounded, OnIsGrounded);
    }

    private void OnJumpIsTriggered(bool isJumpTriggered)
    {
        if (isJumpTriggered && _isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * _rigidbody.mass * _defaultJumpForce, ForceMode.Impulse);
        }
    }

    private void OnIsGrounded(object[] args)
    {
        _isGrounded = (bool)args[0];
    }
}
