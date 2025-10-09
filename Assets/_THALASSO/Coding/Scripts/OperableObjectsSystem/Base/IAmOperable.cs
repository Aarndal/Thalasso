using UnityEngine;

public interface IAmOperable
{
    bool IsOperable { get; }

    void Operate(Transform @operator);
}

public interface IAmOperable<T> : IAmOperable where T : class
{
    void Operate(Transform @operator, T data = null);
}
