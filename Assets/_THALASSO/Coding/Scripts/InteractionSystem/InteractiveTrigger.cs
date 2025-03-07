using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveTrigger : TriggerBase, IAmInteractive
{
    [SerializeField]
    protected TriggerState _sendTriggerState = TriggerState.Switch;

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
        Trigger(transform.gameObject, _sendTriggerState);
    }

    public override void Trigger(GameObject triggeringGameObject, TriggerState triggerState)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggered?.Invoke(triggeringGameObject, triggerState);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
