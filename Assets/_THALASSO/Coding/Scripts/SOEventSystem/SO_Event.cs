using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Scriptable Objects/Event", fileName = "newSOEvent", order = 1)]
public class SO_Event : ScriptableObject
{
    private readonly List<SOEventListener> listeners = new ();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(SOEventListener _listener)
    {
        listeners.Add(_listener);
    }

    public void UnregisterListener(SOEventListener _listener)
    {
        listeners.Remove(_listener);
    }
}
