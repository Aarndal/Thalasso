using UnityEngine;

public sealed class LineOfSightColliderTrigger : LayerMaskColliderTrigger
{
    [SerializeField]
    private Collider _targetedCollider = default;

    protected override void Start()
    {
        base.Start();
        _targetedCollider.isTrigger = false;
    }

    protected override bool IsValidTrigger(GameObject triggeringObject)
    {
        if (triggeringObject == null || !triggeringObject.activeInHierarchy)
            return false;

        if (!IsInTriggeringLayerMasks(triggeringObject))
            return false;

        LineOfSightChecker lineOfSight = triggeringObject.GetComponentInChildren<LineOfSightChecker>();

        if (lineOfSight == null)
            return false;

        if (!lineOfSight.Check(_targetedCollider.transform))
            return false;

        return true;
    }
}
