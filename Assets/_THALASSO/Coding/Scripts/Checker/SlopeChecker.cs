using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
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
    private Collider _collider = default;
    [SerializeField]
    private LayerMask _groundLayerMasks = default;

    private SphereCollider _mySphereCollider = default;
    private Bounds _bounds = default;
    private bool _isGrounded = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }

    private void Awake()
    {
        _mySphereCollider = _mySphereCollider != null ? _mySphereCollider : GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);
    }

    private void Reset()
    {
        _mySphereCollider = _mySphereCollider != null ? _mySphereCollider : GetComponent<SphereCollider>();

        SetSphereCollider(_mySphereCollider);
    }

    private void Start()
    {
        //_collider.bounds.Expand(-2f * _skinWidth);

        _mySphereCollider.radius = _collider.bounds.extents.z + _skinWidth;

        SetSphereCollider(_mySphereCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        Check(other.transform);
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

            _rigidbody.AddForce(CollideAndSlide(_rigidbody.linearVelocity, _rigidbody.transform.position, 0, false, _rigidbody.linearVelocity), ForceMode.VelocityChange);

            return true;
        }
        return false;
    }

    private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, int depth, bool gravityPass, Vector3 initialVelocity)
    {
        if (depth >= _maxBounces)
            return Vector3.zero;

        float distance = velocity.magnitude + _skinWidth;

        if (Physics.SphereCast(position, _bounds.extents.z, velocity.normalized, out RaycastHit hit, distance, _groundLayerMasks, QueryTriggerInteraction.Ignore))
        {
            Vector3 snapToSurface = velocity.normalized * (hit.distance - _skinWidth);

            if (snapToSurface.sqrMagnitude <= _skinWidth * _skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            Vector3 leftoverVelocity = velocity - snapToSurface;
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            // normal ground or slope
            if (angle <= _maxSlopeAngle)
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

            return snapToSurface + CollideAndSlide(leftoverVelocity, position + snapToSurface, depth + 1, gravityPass, initialVelocity);
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

    private void SetSphereCollider(SphereCollider collider)
    {
        collider.isTrigger = true;
        collider.center = transform.up * (collider.radius - _skinWidth);
    }
}
