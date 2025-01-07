using UnityEngine;

public class Test_GlobalEventSystem : MonoBehaviour
{
    #region Unity MonoBehaviour Methods
    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Game.TestTriggered, OnTestTriggered);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string name = other.gameObject.name;
        object[] data = { name };
        GlobalEventBus.Raise(GlobalEvents.Game.TestTriggered, data);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Game.TestTriggered, OnTestTriggered);
    }
    #endregion

    private void OnTestTriggered(params object[] args)
    {
        Debug.Log($"Test Triggered: {args[0]}");
    }
}
