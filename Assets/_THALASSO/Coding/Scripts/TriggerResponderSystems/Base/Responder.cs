using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Responder : MonoBehaviour, IAmResponsive
{
    [Header("Responder Settings")]
    [SerializeField]
    protected ResponderState _startState = ResponderState.Off;
    [SerializeField]
    protected List<MySerializableInterface<IAmTriggerable>> _triggers = new();

    #region Unity Lifecycle Methods
    protected virtual void Awake() => ValidateTriggers();

    protected virtual void OnEnable()
    {
        foreach (var trigger in _triggers)
        {
            trigger.Interface.IsTriggeredBy += OnIsTriggeredBy;
            trigger.Interface.CannotBeTriggered += OnCannotBeTriggered;
        }
    }

    protected virtual void OnDisable()
    {
        foreach (var trigger in _triggers)
        {
            trigger.Interface.CannotBeTriggered -= OnCannotBeTriggered;
            trigger.Interface.IsTriggeredBy -= OnIsTriggeredBy;
        }
    }
    #endregion

    public abstract void Respond(GameObject triggeringObject, ResponderState responderState);

    protected virtual void OnCannotBeTriggered(GameObject triggerObject, string messageText) { }

    protected virtual void OnIsTriggeredBy(GameObject triggerObject, ResponderState responderState, GameObject triggeringObject) => Respond(triggeringObject, responderState);

    protected bool ValidateTriggers()
    {
        if (_triggers.Count <= 0)
        {
            Debug.LogWarningFormat("{3} <color=cyan>{0} (ID: {1})</color> <color=yellow>has no triggers assigned!</color>", gameObject.name, GetInstanceID(), this);
            return false;
        }

        foreach (var trigger in _triggers)
        {
            if(trigger.Interface == null)
            {
                Debug.LogErrorFormat("{3} <color=cyan>{0} (ID: {1})</color> <color=red>has a null trigger assigned!</color>", gameObject.name, GetInstanceID(), this);
            }
        }

        return true;
    }
}
