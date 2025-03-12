using System;
using UnityEngine;

[Serializable]
public abstract class Trigger : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected bool _isTriggerable = true;
    [SerializeField]
    protected bool _isOneTimeTrigger = false;
    [SerializeField, TextArea]
    protected string _cannotBeTriggeredMessage = "";

    protected bool _hasBeenTriggered;

    protected Action<GameObject, string> _cannotBeTriggered;
    protected Action<GameObject, ResponderState> _isTriggered;

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
    public event Action<GameObject, ResponderState> IsTriggered
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
   
    public abstract void ActivateTrigger(GameObject @gameObject, ResponderState triggerState);

    public void SwitchIsTriggerable()
    {
        if (_isOneTimeTrigger && _hasBeenTriggered)
            return;

        IsTriggerable = !IsTriggerable;
    }

    protected abstract bool IsValidTrigger(GameObject triggeringGameObject);

    protected void OnIsTriggered(GameObject @gameObject, ResponderState responderState)
    {
        if (_isOneTimeTrigger)
        {
            IsTriggerable = false;
            _hasBeenTriggered = true;
        }
    }
}
