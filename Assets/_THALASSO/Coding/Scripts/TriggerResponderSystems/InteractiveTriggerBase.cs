using System;
using UnityEngine;

public abstract class InteractiveTriggerBase : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    protected bool _isActivatable;

    public bool IsActivatable { get => _isActivatable; protected set => _isActivatable = value; }

    public event Action<IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }

    public event Action<IAmTriggerable> CannotBeTriggered;
    protected Action<IAmTriggerable> _hasBeenTriggered;

    public virtual void Interact(Transform transform)
    {
        if (!Trigger())
            CannotBeTriggered?.Invoke(this);
    }

    public virtual bool Trigger()
    {
        if (!IsActivatable)
            return false;

        _hasBeenTriggered?.Invoke(this);
        return true;
    }
}
