using UnityEngine;

public class ProximityDoorTrigger : MonoBehaviour
{
    [SerializeField]
    private DoorAnimation doorAnimation;

    private void Start()
    {
        if (doorAnimation == null)
            doorAnimation = GetComponentInChildren<DoorAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        doorAnimation.TryOpen();
    }
    private void OnTriggerExit(Collider other)
    {
        doorAnimation.TryClose();
    }
}
