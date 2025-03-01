using System.Collections.Generic;
using UnityEngine;

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
            trigger.Interface.HasBeenTriggered += OnHasBeenTriggered;
        }
    }

    protected virtual void OnDisable()
    {
        foreach (var trigger in _triggers)
        {
            trigger.Interface.CannotBeTriggered -= OnCannotBeTriggered;
            trigger.Interface.HasBeenTriggered -= OnHasBeenTriggered;
        }
    }

    protected virtual void OnValidate() => ValidateTriggers();
    #endregion

    private void ValidateTriggers()
    {
        if (_triggers.Count <= 0)
            Debug.LogWarningFormat("<color=yellow>Responder</color> {0} (ID: {1}) <color=yellow>has no triggers assigned!</color>", gameObject.name, GetInstanceID());
    }

    protected virtual void OnCannotBeTriggered(GameObject @gameObject, string messageText) { }

    protected virtual void OnHasBeenTriggered(IAmTriggerable trigger) => Respond(trigger);

    public abstract void Respond(IAmTriggerable trigger);
}
