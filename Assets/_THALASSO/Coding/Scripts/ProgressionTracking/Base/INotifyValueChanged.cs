using System;

internal interface INotifyValueChanged<T1, T2>
{
    public event Action<T1, T2> ValueChanged;
}