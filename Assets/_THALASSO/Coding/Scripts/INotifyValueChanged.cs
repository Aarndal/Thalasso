using System;

public interface INotifyValueChanged<T>
{
    uint ID { get; }

    event Action<uint, T> ValueChanged;
}