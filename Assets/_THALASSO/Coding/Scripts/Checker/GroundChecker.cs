using AK.Wwise;
using UnityEngine;

public class GroundChecker : MonoBehaviour, IMakeChecks
{
    [SerializeField]
    private LayerMask _groundLayerMasks = default;
    [SerializeField]
    private Vector3 _groundCheckOffset = Vector3.zero;
    [SerializeField]
    private float _groundCheckRadius = 0.1f;
    [SerializeField]
    private Switch _defaultSoundMaterial;

    private bool _isActive = true;
    private bool _isGrounded = true;
    private Switch _currentSoundMaterial;

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
    public Switch SoundMaterial 
    { 
        get => _currentSoundMaterial; 
        private set 
        { 
            if(_currentSoundMaterial != value)
            {
                _currentSoundMaterial = value;
                GlobalEventBus.Raise(GlobalEvents.Player.GroundSoundMaterialChanged, _currentSoundMaterial);
            }
        } 
    }

    private void Start()
    {
        SoundMaterial = _defaultSoundMaterial;
        GlobalEventBus.Raise(GlobalEvents.Player.GroundedStateChanged, _isGrounded);
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
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];

        IsGrounded = Physics.OverlapSphereNonAlloc(transform.position + _groundCheckOffset, _groundCheckRadius, hitColliders, _groundLayerMasks, QueryTriggerInteraction.Ignore) > 0;

        if (IsGrounded)
        {
            if (Physics.Raycast(transform.position + _groundCheckOffset, -transform.up, out RaycastHit hitInfo, _groundCheckRadius, _groundLayerMasks, QueryTriggerInteraction.Ignore))
                if (hitInfo.collider.TryGetComponent(out SoundMaterial soundMaterial))
                    SoundMaterial = soundMaterial.Get();
        }
    }

}
