using System;
using UnityEngine;

public abstract class ColliderTriggerBase : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected Collider _collider = default;

    public event Action<IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }

    protected Action<IAmTriggerable> _hasBeenTriggered;

    #region Unity Lifecycle Methods
    protected virtual void Awake() => _collider = _collider != null ? _collider : GetComponent<Collider>();

    protected virtual void Start() => _collider.isTrigger = true;

    protected virtual void OnTriggerEnter(Collider other) => Trigger();
    #endregion

    public abstract void Trigger();
}
