using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Event", fileName = "newSOEvent", order = 1)]
public class SO_Event : ScriptableObject
{
    private readonly List<SOEventListener> _eventListeners = new ();

    public void Raise()
    {
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
            _eventListeners[i].RaiseEvent();
    }

    public void RegisterListener(SOEventListener _listener)
    {
        _eventListeners.Add(_listener);
    }

    public void DeregisterListener(SOEventListener _listener)
    {
        _eventListeners.Remove(_listener);
    }
}
