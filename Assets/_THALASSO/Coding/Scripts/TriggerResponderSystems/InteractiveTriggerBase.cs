using System;
using UnityEngine;

public abstract class InteractiveTriggerBase : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    protected bool _isActivatable;

    public bool IsActivatable { get => _isActivatable; protected set => _isActivatable = value; }

    public event Action<IAmTriggerable> CannotBeTriggered;
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

    public virtual void Interact(Transform transform) => Trigger();

    public virtual void Trigger()
    {
        if (!IsActivatable)
        {
            CannotBeTriggered?.Invoke(this);
            return;
        }

        _hasBeenTriggered?.Invoke(this);
    }
}
