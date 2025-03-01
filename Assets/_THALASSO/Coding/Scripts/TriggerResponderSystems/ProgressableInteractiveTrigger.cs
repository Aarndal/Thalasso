using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    public override bool ChangeIsTriggerable() => IsTriggerable = _progressionTracker.IsCompleted;
}
