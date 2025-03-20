using System;
using UnityEngine;

public abstract class SettingElement<T> : MonoBehaviour, IAmSettable, INotifyValueChanged<T>
{
    public uint ID => (uint)GetInstanceID();

    protected Action<uint, T> _valueChanged;

    public event Action<uint, T> ValueChanged
    {
        add
        {
            _valueChanged -= value;
            _valueChanged += value;
        }
        remove => _valueChanged -= value;
    }

    protected virtual void OnEnable() => LoadData();
    protected virtual void Start() => AddListener();
    protected virtual void OnDestroy() => RemoveListener();


    public abstract void DeleteData();

    public abstract void LoadData();

    public abstract void SaveData();


    protected virtual void AddListener() => GlobalEventBus.Register(GlobalEvents.UI.MenuClosed, OnMenuClosed);

    protected virtual void RemoveListener() => GlobalEventBus.Deregister(GlobalEvents.UI.MenuClosed, OnMenuClosed);

    protected virtual void OnMenuClosed(object[] eventArgs) => SaveData();


    protected abstract void SetData(T data);
}