using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
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
    protected virtual void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;
    protected virtual void Start() => LoadData();
    protected virtual void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
    #endregion

    #region Data Management Methods
    public abstract void LoadData();
    protected abstract void SetData(T data);
    public abstract void DeleteData();
    #endregion

    #region Callback Functions
    protected void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        if (loadSceneMode == LoadSceneMode.Additive)
            return;

        LoadData();
    }
    #endregion
}