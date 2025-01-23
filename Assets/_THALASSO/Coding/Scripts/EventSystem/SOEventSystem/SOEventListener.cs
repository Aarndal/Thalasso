using UnityEngine;
using UnityEngine.Events;

public class SOEventListener : MonoBehaviour
{
    public SO_Event GameEvent;
    public UnityEvent GameEventRaised;

    private void OnEnable()
    {
        GameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        GameEvent.DeregisterListener(this);
    }

    public void RaiseEvent()
    {
        GameEventRaised?.Invoke();
    }
}
