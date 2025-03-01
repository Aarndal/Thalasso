using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    public override bool ChangeIsTriggerable() => _isTriggerable = _progressionTracker.IsCompleted;
}
