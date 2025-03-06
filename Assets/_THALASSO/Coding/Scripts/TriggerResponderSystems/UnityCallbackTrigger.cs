using UnityEngine;

public class UnityCallbackTrigger : TriggerBase
{
    [SerializeField]
    protected TriggerMode _triggerMode = TriggerMode.None;

    protected Collider _triggerableCollider = default;

    public Collider Collider => _triggerableCollider;

    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        base.Awake();

        if (IsTriggerModeSet(TriggerMode.Awake))
            Trigger(gameObject);

        if ((_triggerMode & TriggerMode.OnTrigger & TriggerMode.OnCollision) != 0)
        {
            if (!TryGetComponent(out _triggerableCollider))
                Debug.LogErrorFormat("{0} has no Collider component attached, but its TriggerMode requires one!", gameObject.name);
        }
    }

    protected virtual void OnEnable()
    {
        if (IsTriggerModeSet(TriggerMode.OnEnable))
            Trigger(gameObject);
    }

    private void Reset()
    {
        _isTriggerable = true;
        _isOneTimeTrigger = false;
    }

    protected virtual void Start()
    {
        if (IsTriggerModeSet(TriggerMode.Start))
            Trigger(gameObject);

        if (_triggerableCollider != null)
        {
            if ((_triggerMode & TriggerMode.OnTrigger) != 0)
            {
                _triggerableCollider.isTrigger = true;
            }

            if ((_triggerMode & TriggerMode.OnCollision) != 0)
            {
                _triggerableCollider.isTrigger = false;
            }
        }
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

    protected virtual void OnDisable()
    {
        if (IsTriggerModeSet(TriggerMode.OnDisable))
            Trigger(gameObject);
    }

    protected override void OnDestroy()
    {
        if (IsTriggerModeSet(TriggerMode.OnDestroy))
            Trigger(gameObject);

        base.OnDestroy();
    }

    #endregion

    public bool IsTriggerModeSet(TriggerMode triggerMode)
    {
        if ((_triggerMode & triggerMode) != 0)
            return true;
        return false;
    }

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
