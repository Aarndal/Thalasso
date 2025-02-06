using UnityEngine;

[DisallowMultipleComponent]
public class DoorColliderTrigger : ColliderTriggerBase
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity _))
            Trigger();
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (!entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                Trigger();
        }
    }

    public override void Trigger() => _hasBeenTriggered?.Invoke(this);
}
