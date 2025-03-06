using UnityEngine;

[DisallowMultipleComponent]
public class DoorColliderTrigger : UnityCallbackTrigger
{
    private void Reset()
    {
        _triggerableCollider = _triggerableCollider != null ? _triggerableCollider : GetComponent<Collider>();
        
        if (_triggerableCollider != null)
            _triggerableCollider.isTrigger = true;
        
        _isTriggerable = true;
        _isOneTimeTrigger = false;
        _triggerMode = TriggerMode.OnTriggerEnter | TriggerMode.OnTriggerStay;
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
