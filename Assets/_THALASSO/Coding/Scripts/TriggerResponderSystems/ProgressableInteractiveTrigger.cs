using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    public new bool IsTriggerable => _isTriggerable = _progressionTracker.IsCompleted;

    protected void Start()
    {
        _isTriggerable = _progressionTracker.IsCompleted;
    }
}
