using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public sealed class GroundChecker : MonoBehaviour, IMakeChecks
{
    [Header("Variables")]
    [SerializeField]
    private bool _isActive = true;
    [SerializeField]
    private LayerMask _groundLayerMasks = default;
    [SerializeField]
    private Vector3 _groundCheckOffset = Vector3.zero;
    [SerializeField]
    private float _groundCheckRadius = 0.1f;

    private bool _isGrounded = true;
    private SphereCollider _sphereCollider = default;

    private readonly Layers _defaultLayer = Layers.IgnoreRaycast;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            if (_isGrounded != value)
            {
                _isGrounded = value;
                GlobalEventBus.Raise(GlobalEvents.Player.GroundedStateChanged, _isGrounded);
            }
        }
    }

    private void Awake()
    {
        gameObject.layer = (int)_defaultLayer;
        _sphereCollider = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();
    }

    private void Start()
    {
        if (_sphereCollider != null)
            SetSphereCollider();

        GlobalEventBus.Raise(GlobalEvents.Player.GroundedStateChanged, _isGrounded);
    }

    private void Reset()
    {
        SetSphereCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            Check(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            Check(other.transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, _groundCheckRadius);
    }

    private void OnValidate()
    {
        _sphereCollider = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();

        if (_sphereCollider != null)
            SetSphereCollider();
    }

    public bool Check(Transform target)
    {
        if (!IsActive)
            return false;

        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];

        return IsGrounded = Physics.OverlapSphereNonAlloc(transform.position + _groundCheckOffset, _groundCheckRadius, hitColliders, _groundLayerMasks, QueryTriggerInteraction.Ignore) > 0;
    }

    private void SetSphereCollider()
    {
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = _groundCheckRadius;
        _sphereCollider.center = _groundCheckOffset;
    }
}
