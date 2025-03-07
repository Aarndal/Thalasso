using System;
using UnityEngine;

public interface IAmTriggerable
{
    bool IsTriggerable { get; }

    event Action<GameObject, string> CannotBeTriggered;
    event Action<GameObject, TriggerState> IsTriggered;

    void Trigger(GameObject @gameObject, TriggerState triggerState);

    void ChangeIsTriggerable();
}
