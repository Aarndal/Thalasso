using UnityEngine;

public interface IAmInteractive
{
    bool IsActivatable { get; }

    void Interact(Transform transform);
}

public interface IAmInteractive<T> : IAmInteractive where T : class
{
    void Interact(Transform transform, T data);
}
