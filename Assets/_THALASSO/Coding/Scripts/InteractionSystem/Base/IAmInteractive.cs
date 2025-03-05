using UnityEngine;

public interface IAmInteractive
{
    bool IsActivatable { get; }

    void Interact(Transform transform);
}
