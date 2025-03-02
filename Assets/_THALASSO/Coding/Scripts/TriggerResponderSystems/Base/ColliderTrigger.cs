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
public class ColliderTrigger : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected bool _isTriggerable = true;
    [SerializeField]
    protected bool _isOneTimeTrigger = false;
    [SerializeField]
    protected TriggerMode _triggerMode = TriggerMode.None;
    [SerializeField]
    protected string _cannotBeTriggeredMessage = "";

    protected Collider _triggerableCollider;

    #region Properties
    public bool IsTriggerable
    {
        get => _isTriggerable;
        protected set
        {
            if (value != _isTriggerable)
                _isTriggerable = value;
        }
    }
    public Collider Collider => _triggerableCollider;
    #endregion

    #region Public Events
    public event Action<GameObject, string> CannotBeTriggered
    {
        add
        {
            _cannotBeTriggered -= value;
            _cannotBeTriggered += value;
        }
        remove => _cannotBeTriggered -= value;
    }
    public event Action<GameObject, IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }
    #endregion

    protected Action<GameObject, string> _cannotBeTriggered;
    protected Action<GameObject, IAmTriggerable> _hasBeenTriggered;

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

    public virtual bool ChangeIsTriggerable()
    {
        if (_isOneTimeTrigger)
            return IsTriggerable = false;

        return IsTriggerable = !IsTriggerable;
    }

    public bool IsTriggerModeSet(TriggerMode triggerMode)
    {
        if ((_triggerMode & triggerMode) != 0)
            return true;
        return false;
    }

    public virtual void Trigger(GameObject triggeringGameObject)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _hasBeenTriggered?.Invoke(gameObject, this);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected virtual bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
