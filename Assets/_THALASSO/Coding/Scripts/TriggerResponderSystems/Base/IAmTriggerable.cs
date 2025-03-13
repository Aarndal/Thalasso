using System;
using UnityEngine;

public interface IAmTriggerable
{
    bool IsTriggerable { get; }

    event Action<GameObject, string> CannotBeTriggered;
    event Action<GameObject, ResponderState> IsTriggered;

    void ActivateTrigger(GameObject @gameObject, ResponderState responderState);

    void SwitchIsTriggerable();
}
