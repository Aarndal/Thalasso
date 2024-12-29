using UnityEngine;

public class ProximityDoorTrigger : MonoBehaviour
{
    private DoorAnimationTrigger doorAnimationTrigger;
    private void Start()
    {
        doorAnimationTrigger = GetComponent<DoorAnimationTrigger>();
    }
    private void OnTriggerEnter(Collider other)
    {
        doorAnimationTrigger.OpenDoor();
    }
    private void OnTriggerExit(Collider other)
    {
        doorAnimationTrigger.CloseDoor();
    }
}
