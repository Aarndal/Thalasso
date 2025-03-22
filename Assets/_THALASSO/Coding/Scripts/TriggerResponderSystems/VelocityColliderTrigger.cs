using UnityEngine;

public class VelocityColliderTrigger : ColliderTrigger
{
    private void Reset()
    {
        _collider = _collider != null ? _collider : GetComponent<Collider>();
        
        if (_collider != null)
            _collider.isTrigger = true;
        
        _isTriggerable = true;
        _isOneTimeTrigger = false;

        _triggerSettings.TryAdd(ResponderState.TurnOn, (TriggerMode.OnTriggerEnter | TriggerMode.OnTriggerStay));
    }

    protected override bool IsValidTrigger(GameObject triggeringObject)
    {
        if (triggeringObject == null || !triggeringObject.activeInHierarchy)
            return false;

        if (triggeringObject.TryGetComponent(out Entity entity) && !entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return true;

        if (triggeringObject.TryGetComponent(out Rigidbody rigidbody) && rigidbody.linearVelocity.sqrMagnitude > 0.01f) // Checks if the object is moving
            return true;

        return false;
    }
}
