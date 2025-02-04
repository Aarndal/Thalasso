using System.Collections.Generic;
using UnityEngine;

public abstract class ResponderBase : MonoBehaviour, IAmResponsive
{
    protected HashSet<IAmTriggerable> _triggers = new(); //! HashSet to avoid duplicates. Interfaces are not serializable, so we can't use a HashSet directly in the inspector. Needs refactoring!

    protected virtual void Awake()
    {
        if(_triggers.Count == 0)
        {
            var foundTriggersInChildren = GetComponentsInChildren<IAmTriggerable>();

            var foundTriggersInParent = GetComponentsInParent<IAmTriggerable>();

            foreach (var trigger in foundTriggersInChildren)
                _triggers.Add(trigger);

            foreach (var trigger in foundTriggersInParent)
                _triggers.Add(trigger);
        }
    }

    protected virtual void OnEnable()
    {
        foreach (var trigger in _triggers)
            trigger.HasBeenTriggered += OnHasBeenTriggered;
    }

    protected virtual void OnDisable()
    {
        foreach (var trigger in _triggers)
            trigger.HasBeenTriggered -= OnHasBeenTriggered;
    }

    protected virtual void OnHasBeenTriggered(IAmTriggerable trigger) => Respond(trigger);

    public abstract bool Respond(IAmTriggerable trigger);
}
