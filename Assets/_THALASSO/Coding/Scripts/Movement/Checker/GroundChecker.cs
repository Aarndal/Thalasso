using UnityEngine;

public class GroundChecker : MonoBehaviour, IAmChecker
{
    [SerializeField]
    private LayerMask _groundLayerMask;
    [SerializeField]
    private Vector3 _groundCheckOffset = Vector3.zero;
    [SerializeField]
    private float _groundCheckRadius = 0.1f;
    [SerializeField]
    private float _groundCheckDistance = 0.1f;

    private bool _isActive = true;
    private bool _isGrounded = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            if (_isGrounded != value)
            {
                _isGrounded = value;
                GlobalEventBus.Raise(GlobalEvents.Player.IsGrounded, _isGrounded);
            }
        }
    }

    private void Update()
    {
        if (IsActive)
            Check();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, _groundCheckRadius);
    }

    public void Check()
    {
        IsGrounded = Physics.SphereCastNonAlloc(transform.position + _groundCheckOffset, _groundCheckRadius, Vector3.down, new RaycastHit[1], _groundCheckDistance, _groundLayerMask) > 0;
    }

}
