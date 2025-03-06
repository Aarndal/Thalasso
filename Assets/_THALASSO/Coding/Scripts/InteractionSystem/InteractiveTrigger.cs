using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveTrigger : TriggerBase, IAmInteractive
{
    private Collider _interactiveCollider = default;

    public bool IsActivatable => IsTriggerable;

    protected virtual void Awake()
    {
        if (gameObject.layer != (int)Layers.InteractiveObject)
            gameObject.layer = (int)Layers.InteractiveObject;

        _interactiveCollider = _interactiveCollider != null ? _interactiveCollider : GetComponent < Collider>();
    }

    private void Reset()
    {
        gameObject.layer = (int)Layers.InteractiveObject;
    }

    public virtual void Interact(Transform transform) => Trigger(transform.gameObject);
}
