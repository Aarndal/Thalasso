using System;
using UnityEngine;

[Serializable]
public class MySerializableInterface<T> : ISerializationCallbackReceiver where T : class
{
    [SerializeField] 
    private MonoBehaviour _monoBehaviourInstance;
    
    public T Interface { get; private set; }

    public void OnBeforeSerialize()
    {
        if (_monoBehaviourInstance != null && _monoBehaviourInstance is not T)
        {
            _monoBehaviourInstance = null;
        }
    }

    public void OnAfterDeserialize()
    {
        Interface = _monoBehaviourInstance as T;
    }
}