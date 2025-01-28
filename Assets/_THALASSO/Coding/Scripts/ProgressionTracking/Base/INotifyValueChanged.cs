using System;

internal interface INotifyValueChanged<T>
{
    uint ID { get; }

    event Action<uint, T> ValueChanged;
}