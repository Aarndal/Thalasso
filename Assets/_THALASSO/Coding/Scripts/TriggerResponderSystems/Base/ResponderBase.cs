using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ResponderBase : MonoBehaviour, IAmResponsive
{
    [SerializeField]
    protected List<MySerializableInterface<IAmTriggerable>> _triggers = new();

    #region Unity Lifecycle Methods
    protected virtual void Awake() => ValidateTriggers();

    protected virtual void OnEnable()
    {
        foreach (var trigger in _triggers)
        {
            trigger.Interface.CannotBeTriggered += OnCannotBeTriggered;
            trigger.Interface.IsTriggered += OnIsTriggered;
        }
    }
    
    protected virtual void OnValidate() => ValidateTriggers();

    protected virtual void OnDisable()
    {
        foreach (var trigger in _triggers)
        {
            trigger.Interface.CannotBeTriggered -= OnCannotBeTriggered;
            trigger.Interface.IsTriggered -= OnIsTriggered;
        }
    }
    #endregion

    private void ValidateTriggers()
    {
        if (_triggers.Count <= 0)
            Debug.LogWarningFormat("<color=yellow>Responder</color> {0} (ID: {1}) <color=yellow>has no triggers assigned!</color>", gameObject.name, GetInstanceID());
    }

    protected virtual void OnCannotBeTriggered(GameObject @gameObject, string messageText) { }

    protected virtual void OnIsTriggered(GameObject triggeringObject, TriggerState triggerState) => Respond(triggeringObject, triggerState);

    public abstract void Respond(GameObject triggeringObject, TriggerState triggerState);
}
