using System;
using UnityEngine;

public interface IAmTriggerable
{
    bool IsTriggerable { get; }

    event Action<GameObject, string> CannotBeTriggered;
    event Action<GameObject, IAmTriggerable> IsTriggered;

    void Trigger(GameObject gameObject);

    void ChangeIsTriggerable();
}
