using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
public class InteractiveObjectTargetProvider : TargetProvider
{
    [Header("Variables")]
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private float _searchRadius = 10f;
    [SerializeField]
    private int _maxTargetsToSearch = 5;

    private int _numTargetsFound = 0;
    private SphereCollider _sphereCollider = default;
    private Collider[] _targetsInSearchRadius;
    private List<Collider> _closestTargets;
    private List<IAmInteractive> _closestInteractives;

    public float SearchRadius => _searchRadius;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        
        _targetsInSearchRadius = new Collider[_maxTargetsToSearch];
        _closestTargets = new();
        _closestInteractives = new();
    }

    private void Start()
    {
        _sphereCollider.radius = _searchRadius;
        _sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) => GetTarget();

    private void OnTriggerExit(Collider other) => GetTarget();

    public override Transform GetTarget() => Target = FindClosestInteractiveObject();

    private Transform FindClosestInteractiveObject()
    {
        _closestTargets.Clear();

        _numTargetsFound = Physics.OverlapSphereNonAlloc(transform.position, _searchRadius, _targetsInSearchRadius, _layerMask, QueryTriggerInteraction.Collide);
        
        for (int i = 0; i < _numTargetsFound; i++)
        {
            if (_targetsInSearchRadius[i].gameObject.TryGetComponent<IAmInteractive>(out IAmInteractive component))
                _closestTargets.Add(_targetsInSearchRadius[i]);
        }

        float distanceToClosestPlayer = float.MaxValue;
        Transform closestPlayer = null;

        //for (int i = 0; i < _closestTargets.Count; i++)
        //{
        //    if (_closestTargets[i] == null)
        //        continue;

        //    for (int j = 1; j < _tempNavMeshPath.corners.Length; j++)
        //        sqrDistance += Vector3.SqrMagnitude(_tempNavMeshPath.corners[j] - _tempNavMeshPath.corners[j - 1]);

        //    if (sqrDistance >= distanceToClosestPlayer)
        //        continue;

        //    distanceToClosestPlayer = sqrDistance;
        //    closestPlayer = _closestTargets[i].transform;
        //}

        return closestPlayer;
    }
}
