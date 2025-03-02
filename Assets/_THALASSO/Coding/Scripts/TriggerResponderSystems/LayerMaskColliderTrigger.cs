using UnityEngine;

public class LayerMaskColliderTrigger : ColliderTrigger
{
    [SerializeField]
    private LayerMask _triggeringLayerMasks = default;

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => IsInTriggeringLayerMasks(triggeringGameObject);

    protected bool IsInTriggeringLayerMasks(GameObject triggeringGameObject)
    {
        LayerMask layerMask = 1 << triggeringGameObject.layer;

        if ((layerMask & _triggeringLayerMasks) != 0)
            return true;

        return false;
    }
}
