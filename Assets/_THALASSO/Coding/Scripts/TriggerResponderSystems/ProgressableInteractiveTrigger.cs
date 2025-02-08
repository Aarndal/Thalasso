using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    protected override bool CheckTriggerable() => _isTriggerable = _progressionTracker.IsCompleted;
}
