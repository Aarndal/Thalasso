﻿using ProgressionTracking;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ProgressableDoorColliderTrigger : DoorColliderTrigger
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker = default;

    private void Reset()
    {
        _triggerableCollider = _triggerableCollider != null ? _triggerableCollider : GetComponent<Collider>();

        if (_triggerableCollider != null)
            _triggerableCollider.isTrigger = true;

        _isTriggerable = false;
        _isOneTimeTrigger = false;
        _triggerMode = TriggerMode.OnTriggerEnter | TriggerMode.OnTriggerStay;
    }

    public override void Trigger(GameObject triggeringGameObject)
    {
        if (_progressionTracker.IsCompleted != IsTriggerable)
            ChangeIsTriggerable();

        base.Trigger(triggeringGameObject);
    }
}
