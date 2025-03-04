using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class SlopeChecker : MonoBehaviour, IMakeChecks
{
    [SerializeField]
    private bool _isActive = true;
    [SerializeField]
    private float _maxSlopeAngle = 60f;
    [SerializeField]
    private int _maxBounces = 5;
    [SerializeField]
    private float _skinWidth = 0.015f;
    [SerializeField]
    private Rigidbody _rigidbody = default;
    [SerializeField]
    private Collider _bodyCollider = default;
    [SerializeField]
    private LayerMask _groundLayerMasks = default;

    private Collider _myCollider = default;
    private bool _isGrounded = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public Vector3 Velocity { get; private set; }

    public event Action<bool> SlopeDetected
    {
        add
        {
            _slopeDetected -= value;
            _slopeDetected += value;
        }
        remove
        {
            _slopeDetected -= value;
        }
    }

    private Action<bool> _slopeDetected;

    private void Awake()
    {
        _myCollider = _myCollider != null ? _myCollider : GetComponent<Collider>();
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
    }

    private void Start()
    {
        //_collider.bounds.Expand(-2f * _skinWidth);

        _myCollider.bounds.Expand(2f * _skinWidth);

        SetCollider(_myCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            _slopeDetected?.Invoke(Check(other.transform));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            _slopeDetected?.Invoke(false);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
    }

    public bool Check(Transform target)
    {
        if (IsActive)
        {
            LayerMask targetLayerMask = 1 << target.gameObject.layer;

            if ((_groundLayerMasks & targetLayerMask) == 0)
                return false;

            Velocity = CollideAndSlide(_rigidbody.linearVelocity, transform.position + Vector3.up * _skinWidth, 0, false, _rigidbody.linearVelocity);

            if (Velocity != _rigidbody.linearVelocity)
                return true;
        }
        return false;
    }

    private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, int depth, bool gravityPass, Vector3 initialVelocity)
    {
        if (depth >= _maxBounces)
            return Vector3.zero;

        float distance = velocity.magnitude + _skinWidth;

        if (Physics.SphereCast(position + _rigidbody.transform.forward * _myCollider.bounds.extents.x, _myCollider.bounds.extents.x, velocity.normalized, out RaycastHit hit, distance, _groundLayerMasks, QueryTriggerInteraction.Ignore))
        {
            Vector3 snapToSurface = velocity.normalized * (hit.distance - _skinWidth);

            if (snapToSurface.sqrMagnitude <= _skinWidth * _skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            Vector3 leftoverVelocity = velocity - snapToSurface;
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

            // normal ground or slope
            if (slopeAngle <= _maxSlopeAngle)
            {
                if (gravityPass)
                    return snapToSurface;

                leftoverVelocity = Vector3.ProjectOnPlane(leftoverVelocity, hit.normal);
            }
            // wall or steep slope
            else
            {
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                    -new Vector3(initialVelocity.x, 0, initialVelocity.z).normalized);

                if (_isGrounded && !gravityPass)
                {
                    leftoverVelocity = ProjectOnPlaneAndScale(
                        new Vector3(leftoverVelocity.x, 0, leftoverVelocity.z),
                        new Vector3(hit.normal.x, 0, hit.normal.z).normalized);

                    leftoverVelocity *= scale;
                }
                else
                {
                    leftoverVelocity = ProjectOnPlaneAndScale(leftoverVelocity, hit.normal) * scale;
                }
            }

            return snapToSurface + CollideAndSlide(leftoverVelocity, position + snapToSurface, depth++, gravityPass, initialVelocity);
        }

        return velocity;
    }

    private void OnGroundedStateChanged(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is bool isGrounded)
                _isGrounded = isGrounded;
        }
    }

    private Vector3 ProjectOnPlaneAndScale(Vector3 vector, Vector3 planeNormal)
    {
        float magnitude = vector.magnitude;
        vector = Vector3.ProjectOnPlane(vector, planeNormal).normalized;
        return vector *= magnitude;
    }

    private void SetCollider(Collider collider)
    {
        collider.isTrigger = true;
    }
}
