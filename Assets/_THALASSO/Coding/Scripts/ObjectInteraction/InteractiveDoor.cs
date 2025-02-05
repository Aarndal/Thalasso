using UnityEngine;

public class InteractiveDoor : MonoBehaviour, IAmInteractive
{
    [SerializeField]
    private DoorAnimation _doorAnimation = default;

    public bool IsActivatable => _doorAnimation.IsLocked;

    protected virtual void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");

        if (_doorAnimation == null)
            _doorAnimation = GetComponentInParent<DoorAnimation>();
    }

    public void Interact(Transform transform)
    {
        if (IsActivatable)
            return;

        _doorAnimation.OpenDoor();
    }
}
