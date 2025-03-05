using System;
using UnityEngine;

[Flags]
public enum TriggerMode
{
    None = 0,
    OnCollisionEnter = 1 << 0,
    OnCollisionStay = 1 << 1,
    OnCollisionExit = 1 << 2,
    OnTriggerEnter = 1 << 3,
    OnTriggerStay = 1 << 4,
    OnTriggerExit = 1 << 5,
}

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : TriggerBase
{
    [SerializeField]
    protected TriggerMode _triggerMode = TriggerMode.None;

    protected Collider _triggerableCollider;

    public Collider Collider => _triggerableCollider;

    #region Unity Lifecycle Methods
    protected virtual void Awake()
    {
        _triggerableCollider = _triggerableCollider != null ? _triggerableCollider : GetComponent<Collider>();
    }

    private void Reset()
    {
        _triggerableCollider = _triggerableCollider != null ? _triggerableCollider : GetComponent<Collider>();

        if (_triggerableCollider != null)
            _triggerableCollider.isTrigger = true;

        _isTriggerable = true;
        _isOneTimeTrigger = false;
    }

    #region Collision CallbackFunctions
    protected void OnCollisionEnter(Collision collision)
    {
        if (!IsTriggerModeSet(TriggerMode.OnCollisionEnter))
            return;
        Trigger(collision.gameObject);
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!IsTriggerModeSet(TriggerMode.OnCollisionStay))
            return;
        Trigger(collision.gameObject);
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (!IsTriggerModeSet(TriggerMode.OnCollisionExit))
            return;
        Trigger(collision.gameObject);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsTriggerModeSet(TriggerMode.OnTriggerEnter))
            return;
        Trigger(other.gameObject);
    }

    protected void OnTriggerStay(Collider other)
    {
        if (!IsTriggerModeSet(TriggerMode.OnTriggerStay))
            return;
        Trigger(other.gameObject);
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!IsTriggerModeSet(TriggerMode.OnTriggerExit))
            return;
        Trigger(other.gameObject);
    }
    #endregion

    #endregion

    public bool IsTriggerModeSet(TriggerMode triggerMode)
    {
        if ((_triggerMode & triggerMode) != 0)
            return true;
        return false;
    }
}
