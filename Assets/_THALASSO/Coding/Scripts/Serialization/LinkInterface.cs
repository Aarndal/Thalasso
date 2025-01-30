using System;
using UnityEngine;

[Serializable]
public class LinkInterface<T> : ISerializationCallbackReceiver where T : class
{
    [SerializeField] private MonoBehaviour m_interfaceMono;
    public T Interface { get; private set; }

    public void OnBeforeSerialize()
    {
        if (m_interfaceMono != null && m_interfaceMono is not T)
        {
            m_interfaceMono = null;
        }
    }

    public void OnAfterDeserialize()
    {
        Interface = m_interfaceMono as T;
    }

}