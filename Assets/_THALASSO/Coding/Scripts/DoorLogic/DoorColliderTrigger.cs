using UnityEngine;

[DisallowMultipleComponent]
public class DoorColliderTrigger : ColliderTriggerBase
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity _))
            Trigger();
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity _))
            Trigger();
    }

    public override void Trigger()
    {
        _hasBeenTriggered?.Invoke(this);
    }
}
