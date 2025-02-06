using UnityEngine;

public class InteractiveDoorTrigger : InteractiveTriggerBase
{
    [SerializeField]
    private DoorAnimation _doorAnimation = default;

    protected override void Awake()
    {
        base.Awake();

        if (_doorAnimation == null)
            _doorAnimation = GetComponentInParent<DoorAnimation>();
    }

    public override void Interact(Transform transform)
    {
        _isActivatable = !_doorAnimation.IsLocked;
        Trigger();
    }

    public override void Trigger()
    {
        if (!IsActivatable)
            return;

        if (_doorAnimation.TryOpen())
            return;

        if (_doorAnimation.TryClose())
            return;

        _hasBeenTriggered?.Invoke(this);
    }
}
