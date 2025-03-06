using System;

[Flags]
public enum TriggerMode
{
    None = 0,
    Awake = 1 << 0,
    OnEnable = 1 << 1,
    Start = 1 << 2,
    OnTriggerEnter = 1 << 3,
    OnTriggerStay = 1 << 4,
    OnTriggerExit = 1 << 5,
    OnCollisionEnter = 1 << 6,
    OnCollisionStay = 1 << 7,
    OnCollisionExit = 1 << 8,
    OnDisable = 1 << 9,
    OnDestroy = 1 << 10,

    OnTrigger = (OnTriggerEnter | OnTriggerStay | OnTriggerExit),
    OnCollision = (OnCollisionEnter | OnCollisionStay | OnCollisionExit),
}
