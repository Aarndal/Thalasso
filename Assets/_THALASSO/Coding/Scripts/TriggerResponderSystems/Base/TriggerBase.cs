using System;
using UnityEngine;

[Serializable]
public abstract class TriggerBase : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected bool _isTriggerable = true;
    [SerializeField]
    protected bool _isOneTimeTrigger = false;
    [SerializeField]
    protected string _cannotBeTriggeredMessage = "";

    protected bool _hasBeenTriggered;

    protected Action<GameObject, string> _cannotBeTriggered;
    protected Action<GameObject, TriggerState> _isTriggered;

    #region Properties
    public bool IsOneTimeTrigger => _isOneTimeTrigger;

    public bool IsTriggerable
    {
        get => _isTriggerable;
        protected set
        {
            if (value != _isTriggerable)
                _isTriggerable = value;
        }
    }
    #endregion

    #region Public Events
    public event Action<GameObject, string> CannotBeTriggered
    {
        add
        {
            _cannotBeTriggered -= value;
            _cannotBeTriggered += value;
        }
        remove => _cannotBeTriggered -= value;
    }
    public event Action<GameObject, TriggerState> IsTriggered
    {
        add
        {
            _isTriggered -= value;
            _isTriggered += value;
        }
        remove => _isTriggered -= value;
    }
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Awake()
    {
        IsTriggered += OnIsTriggered;
    }
    
    protected virtual void OnDestroy()
    {
        IsTriggered -= OnIsTriggered;
    }
    #endregion

    public void ChangeIsTriggerable()
    {
        if (_isOneTimeTrigger && _hasBeenTriggered)
            return;

        IsTriggerable = !IsTriggerable;
    }

    public abstract void Trigger(GameObject @gameObject, TriggerState triggerState);

    protected abstract bool IsValidTrigger(GameObject triggeringGameObject);

    protected virtual void OnIsTriggered(GameObject triggeringObject, TriggerState triggerState)
    {
        if (_isOneTimeTrigger && triggerState != TriggerState.Pending)
        {
            IsTriggerable = false;
            _hasBeenTriggered = true;
        }
    }
}
