using ProgressionTracking;
using UnityEngine;

[DisallowMultipleComponent]
public class ProgressableDoorColliderTrigger : DoorColliderTrigger
{
    [SerializeField, TextArea]
    protected string _triggerMessage = "";
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    public override void Trigger()
    {
        if (IsTriggerable)
            base.Trigger();
        else
            _cannotBeTriggered?.Invoke(gameObject, _triggerMessage);
    }

    protected override bool CheckTriggerable() => _isTriggerable = _progressionTracker.IsCompleted;
}
