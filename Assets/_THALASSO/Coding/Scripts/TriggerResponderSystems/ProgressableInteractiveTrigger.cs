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
        if (_progressionTracker.IsCompleted != IsTriggerable)
            SwitchIsTriggerable();

        base.ActivateTrigger(triggeringGameObject, responderState);
    }
}
