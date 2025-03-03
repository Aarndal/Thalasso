using ProgressionTracking;
using UnityEngine;

public class ProgressableInteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
    }

    public override void Trigger(GameObject triggeringGameObject)
    {
        if (_progressionTracker.IsCompleted != IsTriggerable)
            ChangeIsTriggerable();

        base.Trigger(triggeringGameObject);
    }
}
