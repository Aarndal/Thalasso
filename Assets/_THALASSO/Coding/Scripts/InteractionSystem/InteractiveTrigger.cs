using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveTrigger : TriggerBase, IAmInteractive
{
    private Collider _interactiveCollider = default;

    public bool IsActivatable => IsTriggerable;

    protected virtual void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");

        _interactiveCollider = _interactiveCollider != null ? _interactiveCollider : GetComponent < Collider>();
    }

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    public virtual void Interact(Transform transform) => Trigger(transform.gameObject);
}
