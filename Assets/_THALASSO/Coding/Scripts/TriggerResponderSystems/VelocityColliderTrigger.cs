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

    protected override bool IsValidTrigger(GameObject triggeringGameObject)
    {
        if (triggeringGameObject.TryGetComponent(out Entity entity) && !entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return true;

        if (triggeringGameObject.TryGetComponent(out Rigidbody rigidbody) && rigidbody.linearVelocity.sqrMagnitude >= 0.001f)
            return true;

        return false;
    }
}
