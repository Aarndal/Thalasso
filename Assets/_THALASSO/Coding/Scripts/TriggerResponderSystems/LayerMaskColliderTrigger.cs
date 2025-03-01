using UnityEngine;

public class LayerMaskColliderTrigger : ColliderTrigger
{
    [SerializeField]
    private LayerMask _triggeringLayerMasks = default;

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => IsInTargetLayerMask(triggeringGameObject);

    protected bool IsInTargetLayerMask(GameObject targetObject)
    {
        LayerMask targetLayerMask = 1 << targetObject.layer;

        if ((targetLayerMask & _triggeringLayerMasks) != 0)
            return true;

        return false;
    }
}
