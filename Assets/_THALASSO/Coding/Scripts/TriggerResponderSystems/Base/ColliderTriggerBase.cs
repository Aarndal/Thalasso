using System;
using UnityEngine;

public abstract class ColliderTriggerBase : MonoBehaviour, IAmTriggerable
{
    [SerializeField]
    protected bool _isTriggerable = true;
    [SerializeField]
    protected Collider _collider = default;

    public bool IsTriggerable => _isTriggerable;

    public event Action<GameObject, string> CannotBeTriggered
    {
        add
        {
            _cannotBeTriggered -= value;
            _cannotBeTriggered += value;
        }
        remove => _cannotBeTriggered -= value;
    }
    public event Action<IAmTriggerable> HasBeenTriggered
    {
        add
        {
            _hasBeenTriggered -= value;
            _hasBeenTriggered += value;
        }
        remove => _hasBeenTriggered -= value;
    }

    protected Action<GameObject, string> _cannotBeTriggered;
    protected Action<IAmTriggerable> _hasBeenTriggered;

    #region Unity Lifecycle Methods
    protected virtual void Awake() => _collider = _collider != null ? _collider : GetComponent<Collider>();

    protected virtual void Start() => _collider.isTrigger = true;

    protected virtual void OnTriggerEnter(Collider other) => Trigger();
    #endregion

    public abstract void Trigger();
}
