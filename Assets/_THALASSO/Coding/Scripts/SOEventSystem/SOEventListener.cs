using UnityEngine;
using UnityEngine.Events;

public class SOEventListener : MonoBehaviour
{
    public SO_Event gameEvent;
    public UnityEvent onEventRaised;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        onEventRaised?.Invoke();
    }
}