using System;
using UnityEngine;

public class InteractiveTrigger : TriggerBase, IAmInteractive
{
    protected virtual void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    public virtual void Interact(Transform transform) => Trigger(transform.gameObject);
}
