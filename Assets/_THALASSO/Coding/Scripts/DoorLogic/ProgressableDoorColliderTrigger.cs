using ProgressionTracking;
using UnityEngine;

public sealed class ProgressableDoorColliderTrigger : DoorColliderTrigger
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker = default;

    private void Reset()
    {
        _collider = _collider != null ? _collider : GetComponent<Collider>();

        if (_collider != null)
            _collider.isTrigger = true;

        _isTriggerable = false;
        _isOneTimeTrigger = false;
        _triggerSettings.TryAdd(ResponderState.TurnOn, (TriggerMode.OnTriggerEnter | TriggerMode.OnTriggerStay));
    }

    public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState triggerState)
    {
        if (_progressionTracker.IsCompleted != IsTriggerable)
            SwitchIsTriggerable();

        base.ActivateTrigger(triggeringGameObject, triggerState);
    }
}
