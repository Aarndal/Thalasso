using System;
using UnityEngine;

public class InteractiveTrigger : MonoBehaviour, IAmTriggerable, IAmInteractive
{
    [SerializeField]
    protected bool _isTriggerable = true;
    [SerializeField, TextArea]
    protected string _cannotBeTriggeredMessage = "";

    public bool IsTriggerable
    {
        get => _isTriggerable;
        protected set
        {
            if (value != _isTriggerable)
                _isTriggerable = value;
        }
    }

    public event Action<GameObject, string> CannotBeTriggered
    {
        add
        {
            _cannotBeTriggered -= value;
            _cannotBeTriggered += value;
        }
        remove => _cannotBeTriggered -= value;
    }
    public event Action<IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }

    protected Action<GameObject, string> _cannotBeTriggered;
    protected Action<IAmTriggerable> _hasBeenTriggered;

    protected virtual void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    public virtual void Interact(Transform transform) => Trigger(transform.gameObject);

    public virtual void Trigger(GameObject gameObject)
    {
        if (IsTriggerable)
            _hasBeenTriggered?.Invoke(this);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    public virtual bool ChangeIsTriggerable() => IsTriggerable = !IsTriggerable;
}
