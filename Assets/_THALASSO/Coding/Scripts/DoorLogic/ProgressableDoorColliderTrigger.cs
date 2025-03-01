using ProgressionTracking;
using UnityEngine;

[DisallowMultipleComponent]
public class ProgressableDoorColliderTrigger : DoorColliderTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    public override bool ChangeIsTriggerable() => IsTriggerable = _progressionTracker.IsCompleted;
}
