using ProgressionTracking;
using System;
using UnityEngine;

public class ProgressableInteractiveTrigger : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker = default;
    [SerializeField, TextArea]
    private string _text;

    public bool IsActivatable => _progressionTracker.IsCompleted;

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
