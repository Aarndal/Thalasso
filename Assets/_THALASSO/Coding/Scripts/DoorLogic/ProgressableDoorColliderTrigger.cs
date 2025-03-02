using ProgressionTracking;
using UnityEngine;

[DisallowMultipleComponent]
public class ProgressableDoorColliderTrigger : DoorColliderTrigger
{
    [SerializeField]
    protected SO_ProgressionTracker _progressionTracker = default;

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
    }

    private void OnProgressionCompleted(object[] args)
    {
        if (args[0] is uint id && id == _progressionTracker.ID)
            ChangeIsTriggerable();
    }

    public override bool ChangeIsTriggerable() => IsTriggerable = _progressionTracker.IsCompleted;

    public override void Trigger(GameObject triggeringGameObject)
    {
        ChangeIsTriggerable();
        base.Trigger(triggeringGameObject);
    }
}
