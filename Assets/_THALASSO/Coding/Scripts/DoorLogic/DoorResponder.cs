using UnityEngine;

public class DoorResponder : ResponderBase
{
    [SerializeField]
    private DoorAnimation _doorAnimation = default;

    private bool _doorIsOpen = false;

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

    protected override void OnEnable()
    {
        base.OnEnable();

        _doorAnimation.HasBeenOpened += () => _doorIsOpen = true;
        _doorAnimation.HasBeenClosed += () => _doorIsOpen = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _doorAnimation.HasBeenOpened -= () => _doorIsOpen = true;
        _doorAnimation.HasBeenClosed -= () => _doorIsOpen = false;
    }

    public override void Respond(IAmTriggerable trigger)
    {
        if (_doorIsOpen)
            _doorAnimation.CloseDoor();
        else
            _doorAnimation.OpenDoor();
    }
}
