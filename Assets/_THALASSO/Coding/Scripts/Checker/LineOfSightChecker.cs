using System;
using UnityEngine;

[DisallowMultipleComponent]
public class LineOfSightChecker : MonoBehaviour, IMakeChecks
{
    [Header("Variables")]
    [SerializeField]
    private bool _isActive = true;
    [SerializeField]
    private LayerMask _ignoredLayerMasks = 1 << 2;
    [SerializeField]
    private float _fieldOfView = 120f; // View angle in degrees
    [SerializeField]
    private float _visionRange = 10f;

    private bool _targetInSight = false;
    private Transform _currentTarget = default;

    public bool IsActive { get => _isActive; set => _isActive = value; }

    public bool TargetInSight
    {
        get => _targetInSight;
        private set
        {
            if (_targetInSight != value)
            {
                _targetInSight = value;
                Debug.Log("Target in Sight: " + _targetInSight);

                if (_targetInSight)
                    GainedSight?.Invoke(CurrentTarget);
                else
                    LostSight?.Invoke(CurrentTarget);
            }
        }
    }

    public Transform CurrentTarget
    {
        get => _currentTarget;
        private set
        {
            if (_currentTarget != value)
                _currentTarget = value;
        }
    }

    public event Action<Transform> GainedSight;
    public event Action<Transform> LostSight;

    public bool Check(Transform target)
    {
        if (!IsActive)
            return _targetInSight = false;

        CurrentTarget = target;

        Ray ray = new()
        {
            origin = transform.position,
            direction = (target.position - transform.position).normalized,
        };

        Debug.DrawRay(ray.origin, ray.direction * _visionRange, TargetInSight ? Color.green : Color.red);

        float cosOfAngleToTarget = Vector3.Dot(transform.forward, ray.direction); // division by magnitudes is not needed because both vectors are normalized => Magnitude = 1

        if (cosOfAngleToTarget >= Mathf.Cos(Mathf.Deg2Rad * _fieldOfView / 2f)) // ">=" because the greater the angle, the smaller the cosine
        {
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, _visionRange, ~_ignoredLayerMasks, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform == target)
                    return _targetInSight = true;
            }
        }

        return _targetInSight = false;
    }
}
