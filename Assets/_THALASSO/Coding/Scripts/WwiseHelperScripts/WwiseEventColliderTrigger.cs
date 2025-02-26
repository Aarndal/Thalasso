using UnityEngine;

public class WwiseEventColliderTrigger : ColliderTriggerBase
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PCAnimation _))
            Trigger();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PCAnimation _))
            Trigger();
    }

    public override void Trigger() => _hasBeenTriggered?.Invoke(this);
}
