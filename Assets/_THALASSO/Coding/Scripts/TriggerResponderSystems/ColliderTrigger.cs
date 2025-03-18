using ProjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class ColliderTrigger : Trigger
{
    [Space(5)]

    [Header("Send ResponderState Settings"), Tooltip("Sets which TriggerMode (right column) should send which ResponderState (left column).")]
    [SerializeField]
    protected SerializableDictionary<ResponderState, TriggerMode> _triggerSettings = default;

    protected Collider _collider = default;

    public Collider Collider => _collider;
    public Dictionary<ResponderState, TriggerMode> TriggerSettings => _triggerSettings;

    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        base.Awake();

        ValidateSettings();

        if (_collider == null)
        {
            if (!TryGetComponent(out _collider))
            {
                _collider = gameObject.AddComponent<Collider>();
            }
        }
    }

    private void Reset()
    {
        _isTriggerable = true;
        _isOneTimeTrigger = false;
    }

    private void OnValidate()
    {
        // Checks if correct TriggerModes are set or if there is a contradiction concerning the Collider component.
        if (TriggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0)
            &&
            TriggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
        {
            Debug.LogErrorFormat("{0} has contradictory statements. A {1} can have either OnTrigger or OnCollision TriggerModes, not both!", gameObject.name, name);
        }
    }

    protected virtual void Start()
    {
        if (_collider != null)
        {
            if (TriggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0))
                _collider.isTrigger = false;

            if (TriggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
                _collider.isTrigger = true;
        }
    }

    #region Collider CallbackFunctions
    protected void OnTriggerEnter(Collider other)
    {
        if (!TryToActivateTrigger(other.gameObject, TriggerMode.OnTriggerEnter))
            return;
    }

    protected void OnTriggerStay(Collider other)
    {
        if (!TryToActivateTrigger(other.gameObject, TriggerMode.OnTriggerStay))
            return;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!TryToActivateTrigger(other.gameObject, TriggerMode.OnTriggerExit))
            return;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (!TryToActivateTrigger(collision.gameObject, TriggerMode.OnCollisionEnter))
            return;
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!TryToActivateTrigger(collision.gameObject, TriggerMode.OnCollisionStay))
            return;
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (!TryToActivateTrigger(collision.gameObject, TriggerMode.OnCollisionExit))
            return;
    }
    #endregion

    #endregion

    /// <summary>
    /// Executes Trigger method if TriggerMode is set in TriggerSettings.
    /// </summary>
    /// <param name="triggeringGameObject">The object that tries to execute the Trigger.</param>
    /// <param name="triggerMode">The TriggerMode that is checked for execution.</param>
    /// <returns>Returns true, if Trigger method was exectued at least once.</returns>
    protected bool TryToActivateTrigger(GameObject triggeringGameObject, TriggerMode triggerMode)
    {
        IEnumerable<KeyValuePair<ResponderState, TriggerMode>> validSettings =
            TriggerSettings.Where(o => (o.Value & triggerMode) != 0);

        if (!validSettings.Any())
            return false;

        foreach (var setting in validSettings)
        {
            ActivateTrigger(triggeringGameObject, setting.Key);
        }

        return true;
    }

    public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState responderState)
    {
        if (!IsValidTrigger(triggeringGameObject))
            return;

        if (IsTriggerable)
            _isTriggeredBy?.Invoke(gameObject, responderState, triggeringGameObject);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }

    protected bool ValidateSettings()
    {
        // Checks if the correct TriggerModes are set to interact with a Collider.
        IEnumerable<KeyValuePair<ResponderState, TriggerMode>> wrongSettings =
            TriggerSettings.Where((o) => (o.Value & (TriggerMode.OnTrigger | TriggerMode.OnCollision)) == 0);

        if (wrongSettings.Any())
        {
            foreach (var setting in wrongSettings)
            {
                Debug.LogErrorFormat("The ResponderState {0} has no TriggerMode to interact with {1}'s Collider!", setting.Key, gameObject.name);
            }
            return false;
        }

        // Checks if there is a contradiction concerning the Collider component.
        if (TriggerSettings.Any((o) => (o.Value & TriggerMode.OnCollision) != 0)
            &&
            TriggerSettings.Any((o) => (o.Value & TriggerMode.OnTrigger) != 0))
        {
            Debug.LogErrorFormat("{0} has contradictory statements. A {1} can have either OnTrigger or OnCollision TriggerModes, not both!", gameObject.name, name);

            return false;
        }

        return true;
    }
}
