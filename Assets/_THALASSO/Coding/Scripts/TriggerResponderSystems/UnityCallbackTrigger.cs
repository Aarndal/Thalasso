using ProjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityCallbackTrigger : Trigger
{
    [Space(5)]

    [Header("Send ResponderState Settings"), Tooltip("Sets which TriggerMode (right column) should send which ResponderState (left column).")]
    [SerializeField]
    protected SerializableDictionary<ResponderState, TriggerMode> _triggerSettings = default;

    public Dictionary<ResponderState, TriggerMode> TriggerSettings => _triggerSettings;

    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        base.Awake();

        TryToActivateTrigger(gameObject, TriggerMode.Awake);
    }

    protected virtual void OnEnable()
    {
        TryToActivateTrigger(gameObject, TriggerMode.OnEnable);
    }

    private void Reset()
    {
        _isTriggerable = true;
        _isOneTimeTrigger = false;
    }

    protected virtual void Start()
    {
        TryToActivateTrigger(gameObject, TriggerMode.Start);
    }

    protected virtual void OnDisable()
    {
        TryToActivateTrigger(gameObject, TriggerMode.OnDisable);
    }

    protected override void OnDestroy()
    {
        TryToActivateTrigger(gameObject, TriggerMode.OnDestroy);

        base.OnDestroy();
    }
    #endregion

    /// <summary>
    /// Activate Trigger if TriggerMode is set in TriggerSettings.
    /// </summary>
    /// <param name="triggeringGameObject">The object that tries to activate the Trigger.</param>
    /// <param name="triggerMode">The TriggerMode that is checked for activate.</param>
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
            _isTriggeredBy?.Invoke(gameObject, responderState, gameObject);
        else
            _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
    }
}
