using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Responder : MonoBehaviour, IAmResponsive
{
    [Header("Responder Settings")]
    [SerializeField]
    protected ResponderState _startState = ResponderState.TurnOff;
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
            Debug.LogErrorFormat("<color=yellow>Responder</color> {0} (ID: {1}) <color=yellow>has no triggers assigned!</color>", gameObject.name, GetInstanceID());
            return false;
        }
        return true;
    }
}
