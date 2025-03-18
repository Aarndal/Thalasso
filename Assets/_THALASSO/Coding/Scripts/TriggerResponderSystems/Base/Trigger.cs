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
    protected Action<GameObject, ResponderState, GameObject> _isTriggeredBy;

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

    public void SwitchIsTriggerable()
    {
        if (_isOneTimeTrigger && _hasBeenTriggered)
            return;

        IsTriggerable = !IsTriggerable;
    }

    protected virtual bool IsValidTrigger(GameObject triggeringObject) => triggeringObject != null && triggeringObject.activeInHierarchy;

    protected virtual void OnIsTriggeredBy(GameObject triggerObject, ResponderState responderState, GameObject triggeringObject)
    {
        if (_isOneTimeTrigger)
        {
            IsTriggerable = false;
            _hasBeenTriggered = true;
        }
    }
}
