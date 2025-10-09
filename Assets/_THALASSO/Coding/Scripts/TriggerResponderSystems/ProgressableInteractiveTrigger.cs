using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    private void Reset()
    {
        gameObject.layer = (int)Layers.InteractiveObject;
    }

    public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState responderState)
    {
        if (_progressionTracker.IsCompleted != IsOperable)
            SwitchTriggerState();

        base.ActivateTrigger(triggeringGameObject, responderState);
    }
}
