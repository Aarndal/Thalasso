using System;
using UnityEngine;

public abstract class InteractiveTriggerBase : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    protected bool _isActivatable = true;
    [SerializeField, TextArea]
    protected string _text = "";

    public bool IsActivatable => _isActivatable;

    public event Action<IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }

    protected Action<IAmTriggerable> _hasBeenTriggered;

    protected virtual void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    public virtual void Interact(Transform transform) => Trigger();

    public abstract void Trigger();
}
