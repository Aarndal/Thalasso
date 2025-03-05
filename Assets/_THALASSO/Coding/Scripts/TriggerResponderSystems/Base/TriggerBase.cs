using System;
using UnityEngine;

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
    protected Action<GameObject, IAmTriggerable> _isTriggered;

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
    public event Action<GameObject, IAmTriggerable> IsTriggered
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
    protected void OnEnable()
    {
        IsTriggered += OnIsTriggered;
    }
    
    protected void OnDisable()
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

    public virtual void Trigger(GameObject triggeringGameObject)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggered?.Invoke(gameObject, this);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected virtual bool IsValidTrigger(GameObject triggeringGameObject) => true;

    protected virtual void OnIsTriggered(GameObject @object, IAmTriggerable triggerable)
    {
        if (_isOneTimeTrigger)
        {
            IsTriggerable = false;
            _hasBeenTriggered = true;
        }
    }
}
