using UnityEngine;

public sealed class LineOfSightColliderTrigger : LayerMaskColliderTrigger
{
    [SerializeField]
    private Collider _targetedCollider = default;

    private void Start()
    {
        _triggerableCollider.isTrigger = true;
        _targetedCollider.isTrigger = false;
    }

    private void OnEnable()
    {
        HasBeenTriggered += OnHasBeenTriggered;
    }

    private void OnDisable()
    {
        HasBeenTriggered -= OnHasBeenTriggered;
    }

    private void OnHasBeenTriggered(GameObject @gameObject, IAmTriggerable triggerable) => ChangeIsTriggerable();

    protected override bool IsValidTrigger(GameObject triggeringGameObject)
    {
        if (!IsInTriggeringLayerMasks(triggeringGameObject))
            return false;

        LineOfSightChecker lineOfSight = triggeringGameObject.GetComponentInChildren<LineOfSightChecker>();

        if (lineOfSight == null)
            return false;

        if (!lineOfSight.Check(_targetedCollider.transform))
            return false;

        return true;
    }
}
