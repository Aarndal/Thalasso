using System;
using UnityEngine;

public interface IAmTriggerable
{
    bool IsActivatable { get; }

    event Action<GameObject, string> CannotBeActivated;
    event Action<GameObject, ResponderState, GameObject> IsTriggeredBy;

    void ActivateTrigger(GameObject triggeringObject, ResponderState responderState);
}
