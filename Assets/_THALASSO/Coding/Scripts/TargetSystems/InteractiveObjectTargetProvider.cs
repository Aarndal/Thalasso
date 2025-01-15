using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class InteractiveObjectTargetProvider : TargetProvider
{
    [Header("Variables")]
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private int _maxTargetsToSearch = 5;
    [SerializeField]
    private float _sphereCastRadius = 0.5f;
    [SerializeField]
    private float _sphereCastDistance = 1.2f;

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
        _capsuleCollider.direction = 2;
        _capsuleCollider.height = _sphereCastDistance + 2 * _sphereCastRadius;
        _capsuleCollider.radius = _sphereCastRadius;
        _capsuleCollider.center = Vector3.forward * (_capsuleCollider.height / 2);
        _capsuleCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) => GetTarget();
    private void OnTriggerStay(Collider other) => GetTarget();
    private void OnTriggerExit(Collider other) => GetTarget();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.forward * _sphereCastRadius, transform.position + transform.forward * (_sphereCastDistance + _sphereCastRadius * 2));
    }
    #endregion

    public override Transform GetTarget() => Target = GetClosestInteractiveObject();

    private Transform GetClosestInteractiveObject()
    {
        _closestTargets.Clear();

        _numTargetsFound = Physics.SphereCastNonAlloc(transform.position + transform.forward * _sphereCastRadius, _sphereCastRadius, transform.forward, _hitTargets, _sphereCastDistance, _layerMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < _numTargetsFound; i++)
        {
            if (_hitTargets[i].transform.TryGetComponent<IAmInteractive>(out IAmInteractive component))
                _closestTargets.Add(_hitTargets[i].transform);
        }

        Transform closestTarget = null;
        float cosPhiToClosestTarget = 0.0f;
        float sqrDistanceToClosestTarget = _sphereCastDistance * _sphereCastDistance + _sphereCastRadius * _sphereCastRadius;

        Vector3 directionToTarget = Vector3.right;
        float cosPhiToTarget = 0.0f;
        float sqrDistanceToTarget = float.MaxValue;

        for (int i = 0; i < _closestTargets.Count; i++)
        {
            if (_closestTargets[i] == null)
                continue;

            directionToTarget = Vector3.Normalize(_closestTargets[i].position - transform.position);
            cosPhiToTarget = Vector3.Dot(transform.forward, directionToTarget);
            sqrDistanceToTarget = Vector3.SqrMagnitude(directionToTarget);

            if (cosPhiToTarget < cosPhiToClosestTarget)
                continue;

            if (cosPhiToTarget == cosPhiToClosestTarget && sqrDistanceToTarget >= sqrDistanceToClosestTarget)
                continue;

            cosPhiToClosestTarget = cosPhiToTarget;
            sqrDistanceToClosestTarget = sqrDistanceToTarget;
            closestTarget = _closestTargets[i];
        }

        return closestTarget;
    }
}
