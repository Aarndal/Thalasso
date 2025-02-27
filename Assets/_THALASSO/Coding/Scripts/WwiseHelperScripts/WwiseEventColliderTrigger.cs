using UnityEngine;

public sealed class WwiseEventColliderTrigger : ColliderTriggerBase
{
    [SerializeField]
    private LayerMask _targetLayerMask = default;

    [Space(5)]

    [SerializeField]
    private bool _hasLineOfSightCheck = true;
    [SerializeField]
    private LayerMask _ignoredLayerMasks = default;
    [SerializeField]
    private float _fieldOfView = 60.0f;

    protected override void OnTriggerEnter(Collider other)
    {
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

        if ((colliderLayerMask & _targetLayerMask) != 0)
            return true;

        return false;
    }


    private bool IsInLineOfSight(Collider collider)
    {
        Ray ray = new()
        {
            origin = collider.transform.position,
            direction = (gameObject.transform.position - collider.transform.position).normalized,
        };

        float cosOfAngleToTarget = Vector3.Dot(collider.transform.forward, ray.direction); // division by magnitudes is not needed because both vectors are normalized => Magnitude = 1

        if (cosOfAngleToTarget >= Mathf.Cos(Mathf.Deg2Rad * _fieldOfView / 2f)) // ">=" because the greater the angle, the smaller the cosine
        {
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, _collider.bounds.max.magnitude, ~_ignoredLayerMasks, QueryTriggerInteraction.Ignore))
            {
                LayerMask hitLayerMask = 1 << hit.collider.gameObject.layer;

                if ((_targetLayerMask & hitLayerMask) != 0)
                    return true;
            }
        }

        return false;
    }
}
