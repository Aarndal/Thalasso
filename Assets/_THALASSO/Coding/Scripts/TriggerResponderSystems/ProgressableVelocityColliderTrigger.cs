using ProgressionTracking;
using UnityEngine;

public sealed class ProgressableVelocityColliderTrigger : VelocityColliderTrigger
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker = default;

    private void Reset()
    {
        _collider = _collider != null ? _collider : GetComponent<Collider>();

        if (_collider != null)
            _collider.isTrigger = true;

        _isActivatable = false;
        _isOneTimeTrigger = false;
        _triggerSettings.TryAdd(ResponderState.On, (TriggerMode.OnTriggerEnter | TriggerMode.OnTriggerStay));
    }

    public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState triggerState)
    {
        if (_progressionTracker.IsCompleted != IsActivatable)
            SwitchTriggerState();

        base.ActivateTrigger(triggeringGameObject, triggerState);
    }
}
