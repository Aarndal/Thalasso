using UnityEngine;

public class DisableTriggerOnExit : MonoBehaviour
{
    [SerializeField]
    private Collider _trigger;

    AkGameObj _wWiseObject;

    private void Awake()
    {
        _trigger = _trigger != null ? _trigger : GetComponent<Collider>();
    }

    private void Start()
    {
        _trigger.isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Entity>(out _))
            _trigger.enabled = false;
    }
}
