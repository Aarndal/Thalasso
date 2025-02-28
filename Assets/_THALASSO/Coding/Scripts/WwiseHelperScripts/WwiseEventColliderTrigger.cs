using System;
using Unity.Cinemachine;
using UnityEngine;

[Flags]
public enum TriggerMode
{
    None = 0,
    OnColliderEnter     = 1 << 0,
    OnColliderStay      = 1 << 1,
    OnColliderExit      = 1 << 2,
    OnTriggerEnter      = 1 << 3,
    OnTriggerStay       = 1 << 4,
    OnTriggerExit       = 1 << 5,
}

public sealed class WwiseEventColliderTrigger : ColliderTriggerBase
{
    [SerializeField]
    private TriggerMode _triggerMode = TriggerMode.None;
    [SerializeField]
    private LayerMask _triggeringLayerMask = default;

    [Space(20)]

    [SerializeField]
    private bool _hasLineOfSightCheck = true;
    [SerializeField]
    private CinemachineCamera _cinemachineCamera = default;
    [SerializeField]
    private Collider _targetCollider = default;
    [SerializeField]
    private LayerMask _ignoredLayerMasks = default;

    protected override void OnTriggerEnter(Collider other)
    {
        if ((_triggerMode & TriggerMode.OnTriggerEnter) == 0)
            return;

        if (IsInTargetLayerMask(other))
        {
            if (!_hasLineOfSightCheck)
            {
                Trigger();
                return;
            }

            if (IsInLineOfSight(other))
            {
                Trigger();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((_triggerMode & TriggerMode.OnTriggerStay) == 0)
            return;

        if (IsInTargetLayerMask(other))
        {
            if (!_hasLineOfSightCheck)
            {
                Trigger();
                return;
            }

            if (IsInLineOfSight(other))
            {
                Trigger();
            }
        }
    }

    public override void Trigger() => _hasBeenTriggered?.Invoke(this);

    private bool IsInTargetLayerMask(Collider collider)
    {
        LayerMask colliderLayerMask = 1 << collider.gameObject.layer;

        if ((colliderLayerMask & _triggeringLayerMask) != 0)
            return true;

        return false;
    }


    private bool IsInLineOfSight(Collider collider)
    {
        Ray ray = new()
        {
            origin = collider.transform.position,
            direction = (_targetCollider.transform.position - collider.transform.position).normalized,
        };

        RaycastHit[] hits = new RaycastHit[10];

        float cosOfAngleToTarget = Vector3.Dot(collider.transform.forward, ray.direction); // division by magnitudes is not needed because both vectors are normalized => Magnitude = 1

        if (cosOfAngleToTarget >= Mathf.Cos(Mathf.Deg2Rad * _cinemachineCamera.Lens.FieldOfView / 2f)) // ">=" because the greater the angle, the smaller the cosine
        {
            if (Physics.RaycastNonAlloc(ray, hits, _triggerCollider.bounds.size.z, ~_ignoredLayerMasks, QueryTriggerInteraction.Ignore) > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider == _targetCollider)
                        return true;
                }
            }
        }

        return false;
    }
}
