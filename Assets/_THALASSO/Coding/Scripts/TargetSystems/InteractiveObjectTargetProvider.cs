using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class InteractiveObjectTargetProvider : TargetProvider
{
    [Header("Variables")]
    [SerializeField]
    private LayerMask _targetLayerMask;
    [SerializeField]
    private LayerMask _ignoredLayerMasks;
    [SerializeField]
    private int _maxTargetsToSearch = 5;
    [SerializeField]
    private float _sphereCastRadius = 0.5f;
    [SerializeField]
    private float _sphereCastDistance = 1.2f;
    [SerializeField, Range(0.9f, 1.0f)]
    private float _targetThreshold = 0.9f;

    private CapsuleCollider _capsuleCollider = default;
    private RaycastHit[] _hitTargets;
    private int _numTargetsFound = 0;
    private List<Transform> _closestTargets;

    #region Unity MonoBehaviour Methods
    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _hitTargets = new RaycastHit[_maxTargetsToSearch];
        _closestTargets = new();
    }

    private void Start()
    {
        // Configures CapsuleCollider to match the sphere cast parameters
        _capsuleCollider.direction = 2;
        _capsuleCollider.height = _sphereCastDistance + 2 * _sphereCastRadius;
        _capsuleCollider.radius = _sphereCastRadius;
        _capsuleCollider.center = Vector3.forward * (_capsuleCollider.height / 2);
        _capsuleCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAmInteractive>(out _))
            GetTarget();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IAmInteractive>(out _))
            GetTarget();
    }
    private void OnTriggerExit(Collider other)
    {
        GetTarget();
    }

    private void OnDrawGizmos()
    {
        // Draw a red line in the scene view to visualize the sphere/ray cast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.forward * _sphereCastRadius, transform.position + transform.forward * (_sphereCastDistance + _sphereCastRadius * 2));
    }

    private void OnValidate()
    {
        // Ensure the CapsuleCollider is correctly configured when changes are made in the inspector
        if (_capsuleCollider == null)
            _capsuleCollider = GetComponent<CapsuleCollider>();

        _capsuleCollider.direction = 2;
        _capsuleCollider.height = _sphereCastDistance + 2 * _sphereCastRadius;
        _capsuleCollider.radius = _sphereCastRadius;
        _capsuleCollider.center = Vector3.forward * (_capsuleCollider.height / 2);
        _capsuleCollider.isTrigger = true;
    }
    #endregion

    public override Transform GetTarget() => Target = GetClosestInteractiveObject();

    private Transform GetClosestInteractiveObject()
    {
        _closestTargets.Clear();

        Vector3 sphereCastOrigin = transform.position + transform.forward * _sphereCastRadius;
        float sphereCastMaxDistance = _sphereCastDistance;

        // Perform a sphere cast to find potential targets on the target layer
        _numTargetsFound = Physics.SphereCastNonAlloc(sphereCastOrigin, _sphereCastRadius, transform.forward, _hitTargets, sphereCastMaxDistance, _targetLayerMask, QueryTriggerInteraction.Collide);

        if (_numTargetsFound <= 0)
            return null;

        // Filter the results to include only objects that implement the IAmInteractive interface
        for (int i = 0; i < _numTargetsFound; i++)
        {
            if (_hitTargets[i].transform.TryGetComponent<IAmInteractive>(out _))
                _closestTargets.Add(_hitTargets[i].transform);
        }

        Transform closestTarget = null;
        float cosPhiToClosestTarget = 0.0f;
        float sqrDistanceToClosestTarget = float.MaxValue;

        Vector3 directionToTarget = Vector3.right;
        float cosPhiToTarget = 0.0f;
        float sqrDistanceToTarget = float.MaxValue;

        QueryTriggerInteraction queryTriggerInteraction;

        // Iterate through the filtered targets to find the closest one
        for (int i = 0; i < _closestTargets.Count; i++)
        {
            if (_closestTargets[i] == null)
                continue;

            directionToTarget = _closestTargets[i].position - transform.position;
            sqrDistanceToTarget = Vector3.SqrMagnitude(directionToTarget);
            cosPhiToTarget = Vector3.Dot(transform.forward.normalized, directionToTarget.normalized);

            // Determine the query trigger interaction based on whether the target's collider is a trigger
            queryTriggerInteraction = (_closestTargets[i].TryGetComponent<Collider>(out Collider collider) && collider.isTrigger) ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            // Perform a raycast to ensure there are no obstacles between the object and the target
            if (Physics.Raycast(sphereCastOrigin, transform.forward, out RaycastHit hitinfo, sphereCastMaxDistance + _sphereCastRadius, ~_ignoredLayerMasks, queryTriggerInteraction) && hitinfo.transform != _closestTargets[i])
                continue;

            // Skip the target if it doesn't meet the minimum angle
            if (cosPhiToTarget < _targetThreshold) //? Redundant check because of spherecast?
                continue;

            // Skip the target if it's farther away than the current closest target
            if (sqrDistanceToTarget > sqrDistanceToClosestTarget)
                continue;

            // Skip the target if the angle is less favorable than the current closest target
            if (cosPhiToTarget < cosPhiToClosestTarget)
                continue;

            // Update the closest target if the current target is closer and meets the criteria
            cosPhiToClosestTarget = cosPhiToTarget;
            sqrDistanceToClosestTarget = sqrDistanceToTarget;
            closestTarget = _closestTargets[i];
        }

        return closestTarget;
    }
}
