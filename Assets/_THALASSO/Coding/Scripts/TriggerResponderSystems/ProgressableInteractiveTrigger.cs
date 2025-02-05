using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTriggerBase
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker = default;

    public bool IsActivatable => _progressionTracker.IsCompleted;
}
