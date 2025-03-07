using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveTrigger : TriggerBase, IAmInteractive
{
    [SerializeField]
    protected TriggerState _currentTriggerState = TriggerState.TurnOff;

    private Collider _interactiveCollider = default;

    public bool IsActivatable => IsTriggerable;

    protected override void Awake()
    {
        base.Awake();

        if (gameObject.layer != (int)Layers.InteractiveObject)
            gameObject.layer = (int)Layers.InteractiveObject;

        _interactiveCollider = _interactiveCollider != null ? _interactiveCollider : GetComponent < Collider>();
    }

    private void Reset()
    {
        gameObject.layer = (int)Layers.InteractiveObject;
    }

    public virtual void Interact(Transform transform)
    {
        _currentTriggerState = _currentTriggerState == TriggerState.TurnOn ? TriggerState.TurnOff : TriggerState.TurnOn;
        Trigger(transform.gameObject, _currentTriggerState);
    }

    public override void Trigger(GameObject triggeringGameObject, TriggerState triggerState)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggered?.Invoke(gameObject, triggerState);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
