using ProjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityCallbackTrigger : TriggerBase
{
    [SerializeField]
    protected SerializableDictionary<TriggerState, TriggerMode> _triggerSettings = default;

    protected Collider _triggerableCollider = default;

    public Collider Collider => _triggerableCollider;

    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        base.Awake();

        TryToTrigger(gameObject, TriggerMode.Awake);

        // Checks whether a Collider component is needed and is attached in the applicable case.
        if (_triggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger & TriggerMode.OnCollision) != 0))
        {
            if (!TryGetComponent(out _triggerableCollider))
                Debug.LogErrorFormat("{0} has no Collider component attached, but its TriggerMode requires one!", gameObject.name);
        }

        // Checks if correct TriggerModes are set or if there is a contradiction concerning the Collider component.
        if (_triggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0) &&
        _triggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
        {
            Debug.LogErrorFormat("{0} has contradictory statements. A {1} can have either OnTrigger or OnCollision TriggerModes, not both!", gameObject.name, name);
        }
    }

    protected virtual void OnEnable()
    {
        TryToTrigger(gameObject, TriggerMode.OnEnable);
    }

    private void Reset()
    {
        _isTriggerable = true;
        _isOneTimeTrigger = false;
    }

    private void OnValidate()
    {
        // Checks if correct TriggerModes are set or if there is a contradiction concerning the Collider component.
        if (_triggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0) &&
        _triggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
        {
            Debug.LogErrorFormat("{0} has contradictory statements. A {1} can have either OnTrigger or OnCollision TriggerModes, not both!", gameObject.name, name);
        }
    }

    protected virtual void Start()
    {
        TryToTrigger(gameObject, TriggerMode.Start);

        if (_triggerableCollider != null)
        {
            if (_triggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0))
                _triggerableCollider.isTrigger = false;

            if (_triggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
                _triggerableCollider.isTrigger = true;
        }
    }

    #region Collision CallbackFunctions
    protected void OnCollisionEnter(Collision collision)
    {
        if (!TryToTrigger(collision.gameObject, TriggerMode.OnCollisionEnter))
            return;
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!TryToTrigger(collision.gameObject, TriggerMode.OnCollisionStay))
            return;
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (!TryToTrigger(collision.gameObject, TriggerMode.OnCollisionExit))
            return;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!TryToTrigger(other.gameObject, TriggerMode.OnTriggerEnter))
            return;
    }

    protected void OnTriggerStay(Collider other)
    {
        if (!TryToTrigger(other.gameObject, TriggerMode.OnTriggerStay))
            return;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!TryToTrigger(other.gameObject, TriggerMode.OnTriggerExit))
            return;
    }
    #endregion

    protected virtual void OnDisable()
    {
        TryToTrigger(gameObject, TriggerMode.OnDisable);
    }

    protected override void OnDestroy()
    {
        TryToTrigger(gameObject, TriggerMode.OnDestroy);

        base.OnDestroy();
    }

    #endregion

    /// <summary>
    /// Executes Trigger method if TriggerMode is set in TriggerSettings.
    /// </summary>
    /// <param name="triggeringGameObject">The object that tries to execute the Trigger.</param>
    /// <param name="triggerMode">The TriggerMode that is checked for execution.</param>
    /// <returns>Returns true, if Trigger method was exectued at least once.</returns>
    protected bool TryToTrigger(GameObject triggeringGameObject, TriggerMode triggerMode)
    {
        HashSet<TriggerState> triggerStates = new();

        foreach (var triggerSetting in _triggerSettings)
        {
            if ((triggerSetting.Value & triggerMode) != 0)
            {
                triggerStates.Add(triggerSetting.Key);
                Trigger(triggeringGameObject, triggerSetting.Key);
            }
        }

        return triggerStates.Count > 0;
    }

    public override void Trigger(GameObject triggeringGameObject, TriggerState triggerState)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggered?.Invoke(gameObject, triggerState);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected override bool IsValidTrigger(GameObject triggeringGameObject) => true;
}
