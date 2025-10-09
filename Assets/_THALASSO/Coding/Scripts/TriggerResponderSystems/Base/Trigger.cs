using System;
using UnityEngine;

[Serializable]
public abstract class Trigger : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected bool _isActivatable = true;
    [SerializeField]
    protected bool _isOneTimeTrigger = false;
    [SerializeField, TextArea]
    protected string _cannotBeActivatedMessage = "";

    protected bool _hasBeenTriggered;

    protected Action<GameObject, string> _cannotBeActivated;
    protected Action<GameObject, ResponderState, GameObject> _isTriggeredBy;

    #region Properties
    public bool IsOneTimeTrigger => _isOneTimeTrigger;

    public bool IsActivatable
    {
        get => _isActivatable;
        protected set
        {
            if (value != _isActivatable)
                _isActivatable = value;
        }
    }
    #endregion

    #region Public Events
    public event Action<GameObject, string> CannotBeActivated
    {
        add
        {
            _cannotBeActivated -= value;
            _cannotBeActivated += value;
        }
        remove => _cannotBeActivated -= value;
    }
    public event Action<GameObject, ResponderState, GameObject> IsTriggeredBy
    {
        add
        {
            _isTriggeredBy -= value;
            _isTriggeredBy += value;
        }
        remove => _isTriggeredBy -= value;
    }
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Awake()
    {
        IsTriggeredBy += OnIsTriggeredBy;
    }


    protected virtual void OnDestroy()
    {
        IsTriggeredBy -= OnIsTriggeredBy;
    }
    #endregion

    public abstract void ActivateTrigger(GameObject triggeringObject, ResponderState triggerState);


    protected virtual bool IsValidTrigger(GameObject triggeringObject) => triggeringObject != null && triggeringObject.activeInHierarchy;

    protected virtual void OnIsTriggeredBy(GameObject triggerObject, ResponderState responderState, GameObject triggeringObject)
    {
        if (_isOneTimeTrigger)
        {
            IsActivatable = false;
            _hasBeenTriggered = true;
        }
    }

    protected void SwitchTriggerState()
    {
        if (_isOneTimeTrigger && _hasBeenTriggered)
            return;

        IsActivatable = !IsActivatable;
    }
}
