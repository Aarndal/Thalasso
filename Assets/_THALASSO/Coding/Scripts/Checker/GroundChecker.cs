#if WWISE_2024_OR_LATER
using WwiseHelper;
#endif
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class GroundChecker : MonoBehaviour, IMakeChecks
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

#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Switch _defaultSoundMaterial;

    private AK.Wwise.Switch _currentSoundMaterial;
#endif

    private bool _isGrounded = true;
    private SphereCollider _sphereCollider = default;
    private LayerMask _defaultLayerMask = 1 << 2;

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

#if WWISE_2024_OR_LATER
    public AK.Wwise.Switch CurrentSoundMaterial
    {
        get => _currentSoundMaterial;
        private set
        {
            if (_currentSoundMaterial != value)
            {
                _currentSoundMaterial = value;
                GlobalEventBus.Raise(GlobalEvents.Player.GroundSoundMaterialChanged, _currentSoundMaterial);
            }
        }
    }
#endif

    private void Awake()
    {
        gameObject.layer = _defaultLayerMask.value;
        _sphereCollider = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();
    }

    private void Start()
    {
        if (_sphereCollider != null)
            SetSphereCollider();

#if WWISE_2024_OR_LATER
        CurrentSoundMaterial = _defaultSoundMaterial;
#endif
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

        IsGrounded = Physics.OverlapSphereNonAlloc(transform.position + _groundCheckOffset, _groundCheckRadius, hitColliders, _groundLayerMasks, QueryTriggerInteraction.Ignore) > 0;

#if WWISE_2024_OR_LATER
        if (IsGrounded)
        {
            if (Physics.Raycast(transform.position + _groundCheckOffset, -transform.up, out RaycastHit hitInfo, _groundCheckRadius, _groundLayerMasks, QueryTriggerInteraction.Ignore))
                if (hitInfo.collider.TryGetComponent(out SoundMaterial soundMaterial))
                    CurrentSoundMaterial = soundMaterial.Get();
        }
#endif
        return IsGrounded;
    }

    private void SetSphereCollider()
    {
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = _groundCheckRadius;
        _sphereCollider.center = _groundCheckOffset;
    }
}
