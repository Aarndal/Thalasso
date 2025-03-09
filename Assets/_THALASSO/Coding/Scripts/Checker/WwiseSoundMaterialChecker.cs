using UnityEngine;
using WwiseHelper;
using System;
using UnityEditor.PackageManager;

// Checks any hit Collider for an attached SoundMaterial component.
[RequireComponent(typeof(SphereCollider))]
public sealed class WwiseSoundMaterialChecker : MonoBehaviour, IMakeChecks, INotifyValueChanged<AK.Wwise.Switch>
{
    [Header("Variables")]
    [SerializeField]
    private bool _isActive = true;
    [SerializeField]
    private LayerMask _toCheckLayerMasks = default;
    [SerializeField]
    private Vector3 _sphereOffset = Vector3.zero;
    [SerializeField]
    private float _sphereRadius = 0.1f;

#if WWISE_2024_OR_LATER
    private AK.Wwise.Switch _currentSoundMaterial = default;
#endif

    private SphereCollider _sphereCollider = default;
    private readonly Layers _defaultLayerMask = Layers.IgnoreRaycast;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public uint ID => (uint)gameObject.GetInstanceID();

#if WWISE_2024_OR_LATER
    public AK.Wwise.Switch CurrentSoundMaterial
    {
        get => _currentSoundMaterial;
        private set
        {
            if (value != _currentSoundMaterial)
            {
                _currentSoundMaterial = value;
                _valueChanged?.Invoke(ID, _currentSoundMaterial);
            }
        }
    }

    private Action<uint, AK.Wwise.Switch> _valueChanged;

    public event Action<uint, AK.Wwise.Switch> ValueChanged
    {
        add
        {
            _valueChanged -= value;
            _valueChanged += value;
        }
        remove => _valueChanged -= value;
    }
#endif

    #region Unity LifecycleMethods
    private void Awake()
    {
        gameObject.layer = (int)_defaultLayerMask;
        _sphereCollider = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();
    }

    private void Start()
    {
        if (_sphereCollider != null)
            SetSphereCollider();
    }

    private void Reset()
    {
        SetSphereCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        LayerMask hitLayer = 1 << other.gameObject.layer;
        if ((hitLayer & _toCheckLayerMasks) != 0)
            Check(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        LayerMask hitLayer = 1 << other.gameObject.layer;
        if ((hitLayer & _toCheckLayerMasks) != 0)
            if (!Check(other.transform))
                CurrentSoundMaterial = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _sphereOffset, _sphereRadius);
    }

    private void OnValidate()
    {
        _sphereCollider = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();

        if (_sphereCollider != null)
            SetSphereCollider();
    }
    #endregion

    public bool Check(Transform target)
    {
        if (!IsActive)
            return false;

        Collider[] overlappingColliders = new Collider[10];

#if WWISE_2024_OR_LATER
        //if (Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit hitInfo, (target.transform.position - transform.position).magnitude, _toCheckLayerMasks))
        if (Physics.OverlapSphereNonAlloc(transform.position + _sphereOffset, _sphereRadius, overlappingColliders, _toCheckLayerMasks) > 0)
        {
            float distanceToClosestCollider = float.MaxValue;
            WwiseSoundMaterial closestSoundMaterial = null;

            foreach (var collider in overlappingColliders )
            {
                if (collider == null)
                    continue;

                if (collider.TryGetComponent(out WwiseSoundMaterial soundMaterial))
                {
                    float distanceToCollider = (collider.transform.position - transform.position).sqrMagnitude;

                    if (distanceToCollider < distanceToClosestCollider)
                    {
                        closestSoundMaterial = soundMaterial;
                        distanceToClosestCollider = distanceToCollider;
                    }
                }

                //if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out RaycastHit hitInfo, (collider.transform.position - transform.position).magnitude, _toCheckLayerMasks))
                //{
                //    if (hitInfo.collider.TryGetComponent(out WwiseSoundMaterial soundMaterial))
                //    {
                //        CurrentSoundMaterial = soundMaterial.Get();
                //        return true;
                //    }
                //}
            }

            if(closestSoundMaterial != null)
            {
                CurrentSoundMaterial = closestSoundMaterial.Get();
                return true;
            }
        }

        //if (target.gameObject.TryGetComponent(out WwiseSoundMaterial soundMaterial))
        //{
        //    if (CurrentSoundMaterial != soundMaterial.Get())
        //        CurrentSoundMaterial = soundMaterial.Get();
        //    return true;
        //}
#endif
        return false;
    }

    private void SetSphereCollider()
    {
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = _sphereRadius;
        _sphereCollider.center = _sphereOffset;
    }
}

