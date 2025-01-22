using UnityEngine;

public interface IAmInteractive
{
    public bool IsActivatable { get; }

    void Interact(Transform transform);
}
