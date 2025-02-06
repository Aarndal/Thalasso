using System;
using UnityEngine;

public abstract class InteractiveTriggerBase : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    private bool _isActivatable;
    [SerializeField, TextArea]
    private string _text;

    public bool IsActivatable => _isActivatable;

    public event Action<IAmTriggerable, string> CannotBeTriggered;
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
            CannotBeTriggered?.Invoke(this, _text);
            return;
        }

        _hasBeenTriggered?.Invoke(this);
    }
}
