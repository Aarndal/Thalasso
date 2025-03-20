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

    #region Unity Lifecycle Methods
    protected virtual void OnEnable() => AddListener();
    protected virtual void Start() => LoadData();
    protected virtual void OnDestroy() => RemoveListener();
    #endregion

    #region Data Management Methods
    public abstract void LoadData();
    protected abstract void SetData(T data);
    public abstract void SaveData();
    public abstract void DeleteData();
    #endregion

    #region Callback Functions
    protected virtual void AddListener() => GlobalEventBus.Register(GlobalEvents.UI.MenuClosed, OnMenuClosed);
    protected virtual void RemoveListener() => GlobalEventBus.Deregister(GlobalEvents.UI.MenuClosed, OnMenuClosed);
    protected virtual void OnMenuClosed(object[] eventArgs) => SaveData();
    #endregion
}