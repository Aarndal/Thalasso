using System;
using UnityEngine;

public class ColliderTriggerBase : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected Collider _collider = default;

    public event Action<IAmTriggerable> CannotBeTriggered;
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity _))
            Trigger();
    }
    #endregion

    public virtual void Trigger()
    {
        _hasBeenTriggered?.Invoke(this);
    }
}
