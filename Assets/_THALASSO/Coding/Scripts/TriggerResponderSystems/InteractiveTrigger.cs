using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class InteractiveTrigger : Trigger, IAmInteractive
{
    [SerializeField]
    protected ResponderState _triggeringResponderState = ResponderState.Switch;

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
        _triggeringResponderState = ResponderState.Switch;
    }

    public virtual void Interact(Transform transform)
    {
        ActivateTrigger(transform.gameObject, _triggeringResponderState);
    }

    public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState responderState)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggered?.Invoke(triggeringGameObject, responderState);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
