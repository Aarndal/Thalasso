using UnityEngine;

public class LayerMaskColliderTrigger : ColliderTrigger
{
    [SerializeField]
    private LayerMask _triggeringLayerMasks = default;

    protected override bool IsValidTrigger(GameObject triggeringObject)
    {
        if (triggeringObject == null || !triggeringObject.activeInHierarchy)
            return false;

        return IsInTriggeringLayerMasks(triggeringObject);
    }

    protected bool IsInTriggeringLayerMasks(GameObject triggeringObject)
    {
        LayerMask layerMask = 1 << triggeringObject.layer;

        if ((layerMask & _triggeringLayerMasks) != 0)
            return true;

        return false;
    }
}
