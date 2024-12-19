using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game Event", fileName = "newGameEvent", order = 1)]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> listeners = new ();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener _listener)
    {
        listeners.Add(_listener);
    }

    public void UnregisterListener(GameEventListener _listener)
    {
        listeners.Remove(_listener);
    }
}
