using System;
using Unity.Cinemachine;
using UnityEngine;

[Flags]
public enum TriggerMode
{
    None = 0,
    OnColliderEnter = 1 << 0,
    OnColliderStay = 1 << 1,
    OnColliderExit = 1 << 2,
    OnTriggerEnter = 1 << 3,
    OnTriggerStay = 1 << 4,
    OnTriggerExit = 1 << 5,
}

public sealed class WwiseEventColliderTrigger : ColliderTriggerBase
{
    [SerializeField]
    private bool _isOnTimeTrigger = true;
    [SerializeField]
    private TriggerMode _triggerMode = TriggerMode.None;
    [SerializeField]
    private LayerMask _triggeringLayerMask = default;
    [SerializeField]
    private Collider _targetedCollider = default;

    protected override void Awake()
    {
        base.Awake();
        _targetedCollider.isTrigger = false;
    }

    private void OnEnable()
    {
        HasBeenTriggered += OnHasBeenTriggered;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if ((_triggerMode & TriggerMode.OnTriggerEnter) == 0)
            return;

        if (IsInTargetLayerMask(other.transform))
        {
            LineOfSightChecker lineOfSight = other.gameObject.GetComponentInChildren<LineOfSightChecker>();

            if (lineOfSight != null)
            {
                if (lineOfSight.Check(_targetedCollider.transform))
                {
                    Trigger();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((_triggerMode & TriggerMode.OnTriggerStay) == 0)
            return;

        if (IsInTargetLayerMask(other.transform))
        {
            LineOfSightChecker lineOfSight = other.gameObject.GetComponentInChildren<LineOfSightChecker>();

            if (lineOfSight != null)
            {
                if (lineOfSight.Check(_targetedCollider.transform))
                {
                    Trigger();
                }
            }
        }
    }

    private void OnDisable()
    {
        HasBeenTriggered -= OnHasBeenTriggered;
    }

    public override void Trigger()
    {
        if (IsTriggerable)
            _hasBeenTriggered?.Invoke(this);
        else
        {
            _cannotBeTriggered?.Invoke(gameObject, gameObject.name + " has already been triggered.");
            gameObject.SetActive(false);
        }
    }

    private void OnHasBeenTriggered(IAmTriggerable triggerable) => ChangeTriggerable();

    public override bool ChangeTriggerable()
    {
        if (_isOnTimeTrigger)
            return IsTriggerable = false;

        return IsTriggerable = true;
    }

    private bool IsInTargetLayerMask(Transform transform)
    {
        LayerMask colliderLayerMask = 1 << transform.gameObject.layer;

        if ((colliderLayerMask & _triggeringLayerMask) != 0)
            return true;

        return false;
    }
}
