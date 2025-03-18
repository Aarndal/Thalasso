using System;
using UnityEngine;

public interface IAmTriggerable
{
    bool IsTriggerable { get; }

    event Action<GameObject, string> CannotBeTriggered;
    event Action<GameObject, ResponderState, GameObject> IsTriggeredBy;

    void ActivateTrigger(GameObject triggeringObject, ResponderState responderState);

    void SwitchIsTriggerable();
}
