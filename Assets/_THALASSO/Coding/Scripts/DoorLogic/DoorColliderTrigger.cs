using UnityEngine;

[DisallowMultipleComponent]
public class DoorColliderTrigger : ColliderTrigger
{
    protected override bool IsValidTrigger(GameObject triggeringGameObject)
    {
        if (!triggeringGameObject.TryGetComponent(out Entity entity))
            return false;

        if (!entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return true;

        return false;
    }
}
