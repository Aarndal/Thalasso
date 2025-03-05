using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class SlopeChecker : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;
    [SerializeField]
    private CapsuleCollider _bodyCollider = default;

    [SerializeField]
    private bool _isActive = true;
    [SerializeField]
    private float _maxSlopeAngle = 60f;
    [SerializeField]
    private float _skinWidth = 0.015f;
    [SerializeField]
    private float _slopeSpeedBoost = 5.0f;
    [SerializeField]
    private LayerMask _groundLayerMasks = default;

    private SphereCollider _myCollider = default;
    private Collider _slopeCollider = default;
    private RaycastHit[] _hitTargets;
    private Vector3 _initialMoveDirection;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public Vector3 MoveDirection { get; private set; }
    public LayerMask TargetedLayerMasks => _groundLayerMasks;

    public event Action<bool> SlopeDetected
    {
        add
        {
            _slopeDetected -= value;
            _slopeDetected += value;
        }
        remove => _slopeDetected -= value;
    }

    private Action<bool> _slopeDetected;

    private void Awake()
    {
        _myCollider = _myCollider != null ? _myCollider : GetComponent<SphereCollider>();
        _hitTargets = new RaycastHit[10];
    }

    private void OnEnable()
    {
        _input.MoveInputHasChanged += OnMoveInputHasChanged;
    }

    private void Start()
    {
        SetCollider(_myCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (Check(other.transform))
            {
                _slopeCollider = other;
                _slopeDetected?.Invoke(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger && _slopeCollider)
        {
            if (Check(other.transform))
            {
                _slopeDetected?.Invoke(true);
            }
            else
            {
                _slopeDetected?.Invoke(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && _slopeCollider)
        {
            _slopeDetected?.Invoke(false);
        }
    }

    private void OnDisable()
    {
        _input.MoveInputHasChanged -= OnMoveInputHasChanged;
    }

    public bool Check(Transform target)
    {
        if (IsActive)
        {
            LayerMask targetLayerMask = 1 << target.gameObject.layer;

            if ((TargetedLayerMasks & targetLayerMask) == 0)
                return false;

            MoveDirection = CollideAndSlide(target, _initialMoveDirection);

            if (MoveDirection != _initialMoveDirection && MoveDirection != Vector3.zero)
                return true;
        }
        return false;
    }

    private Vector3 CollideAndSlide(Transform target, Vector3 direction)
    {
        if (Physics.SphereCastNonAlloc(transform.position + _skinWidth * 2.0f * Vector3.up, _skinWidth, transform.forward, _hitTargets, (target.position - transform.position).magnitude + _myCollider.radius, _groundLayerMasks, QueryTriggerInteraction.Ignore) > 0)
        {
            RaycastHit hit = default;

            foreach (RaycastHit hitTarget in _hitTargets)
            {
                if (hitTarget.transform == target.transform)
                {
                    hit = hitTarget;
                    break;
                }
            }

            if (hit.transform != null)
            {
                Vector3 snapToSurface = direction.normalized * (hit.distance - _skinWidth);

                if (snapToSurface.sqrMagnitude <= _skinWidth * _skinWidth)
                {
                    snapToSurface = Vector3.zero;
                }

                Vector3 differentialVector = direction - snapToSurface;

                Vector3 normal = hit.normal;

                if (Vector3.Dot(target.transform.up, hit.normal) <= -0.001f)
                    normal *= -1.0f;

                float slopeAngle = Vector3.Angle(Vector3.up, normal);

                // normal ground or slope
                if (slopeAngle <= _maxSlopeAngle)
                {
                    differentialVector = ProjectOnPlaneAndScale(differentialVector, normal);
                }
                else
                {
                    differentialVector = Vector3.zero;
                }
                    
                return snapToSurface + differentialVector;
            }
        }
        return direction;
    }

    //private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, int depth, bool gravityPass, Vector3 initialVelocity)
    //{
    //    if (depth >= _maxBounces)
    //        return Vector3.zero;

    //    float distance = velocity.magnitude + _skinWidth;

    //    if (Physics.SphereCast(position + _rigidbody.transform.forward * _myCollider.bounds.extents.x, _myCollider.bounds.extents.x, velocity.normalized, out RaycastHit hit, distance, _groundLayerMasks, QueryTriggerInteraction.Ignore))
    //    {
    //        Vector3 snapToSurface = velocity.normalized * (hit.distance - _skinWidth);

    //        if (snapToSurface.sqrMagnitude <= _skinWidth * _skinWidth)
    //        {
    //            snapToSurface = Vector3.zero;
    //        }

    //        Vector3 leftoverVelocity = velocity - snapToSurface;
    //        float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

    //        // normal ground or slope
    //        if (slopeAngle <= _maxSlopeAngle)
    //        {
    //            if (gravityPass)
    //                return snapToSurface;

    //            leftoverVelocity = Vector3.ProjectOnPlane(leftoverVelocity, hit.normal);
    //        }
    //        // wall or steep slope
    //        else
    //        {
    //            float scale = 1 - Vector3.Dot(
    //                new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
    //                -new Vector3(initialVelocity.x, 0, initialVelocity.z).normalized);

    //            if (_isGrounded && !gravityPass)
    //            {
    //                leftoverVelocity = ProjectOnPlaneAndScale(
    //                    new Vector3(leftoverVelocity.x, 0, leftoverVelocity.z),
    //                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized);

    //                leftoverVelocity *= scale;
    //            }
    //            else
    //            {
    //                leftoverVelocity = ProjectOnPlaneAndScale(leftoverVelocity, hit.normal) * scale;
    //            }
    //        }

    //        return snapToSurface + CollideAndSlide(leftoverVelocity, position + snapToSurface, depth++, gravityPass, initialVelocity);
    //    }

    //    return velocity;
    //}

    //private void OnGroundedStateChanged(object[] args)
    //{
    //    foreach (var arg in args)
    //    {
    //        if (arg is bool isGrounded)
    //            _isGrounded = isGrounded;
    //    }
    //}

    private void OnMoveInputHasChanged(Vector2 moveInput)
    {
        _initialMoveDirection = new Vector3(moveInput.x, 0, moveInput.y);
    }

    private Vector3 ProjectOnPlaneAndScale(Vector3 vector, Vector3 planeNormal)
    {
        float magnitude = vector.magnitude * _slopeSpeedBoost;
        vector = Vector3.ProjectOnPlane(vector, planeNormal).normalized;
        return vector *= magnitude;
    }

    private void SetCollider(SphereCollider collider)
    {
        collider.center = transform.position + (_bodyCollider.radius + _skinWidth) * Vector3.up;
        collider.radius = _bodyCollider.radius + _skinWidth;
    }
}
