using UnityEngine;

public class DoorResponder : ResponderBase
{
    [SerializeField]
    private DoorAnimation _doorAnimation = default;

    protected override void Awake()
    {
        base.Awake();

        if (_doorAnimation == null)
            if (!TryGetComponent(out _doorAnimation))
                _doorAnimation = GetComponentInChildren<DoorAnimation>();

        if (_doorAnimation == null)
            _doorAnimation = GetComponentInParent<DoorAnimation>();

        if (_doorAnimation == null)
            Debug.LogWarningFormat("<color=yellow>DoorResponder</color> {0} (ID: {1}) <color=yellow>has no DoorAnimation assigned!</color>", gameObject.name, gameObject.GetInstanceID());
    }

    protected override void OnCannotBeTriggered(GameObject @gameObject, string messageText)
    {
        if (!_doorAnimation.IsLocked)
            _doorAnimation.Lock();
    }

    public override void Respond(GameObject @gameObject, IAmTriggerable trigger)
    {
        if (!trigger.IsTriggerable && !_doorAnimation.IsLocked)
        {
            _doorAnimation.Lock();
            return;
        }

        if (trigger.IsTriggerable && _doorAnimation.IsLocked)
            _doorAnimation.Unlock();

        if (_doorAnimation.TryOpen())
            return;

        if (_doorAnimation.TryClose())
            return;
    }
}
