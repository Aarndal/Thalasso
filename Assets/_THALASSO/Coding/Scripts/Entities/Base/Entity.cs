using UnityEngine;

[DisallowMultipleComponent]
public abstract class Entity : MonoBehaviour
{
    protected Animator _animator = default;
    protected AnimationEventBroadcaster _animationEventBroadcaster = default;

    public Animator Animator => _animator;
    public AnimationEventBroadcaster AnimationEventBroadcaster => _animationEventBroadcaster;

    protected virtual void Awake()
    {
        _animator = Animator != null ? Animator : GetComponentInChildren<Animator>();
        _animationEventBroadcaster = AnimationEventBroadcaster != null ? AnimationEventBroadcaster : GetComponentInChildren<AnimationEventBroadcaster>();
    }

    protected virtual void OnEnable()
    {
        if (AnimationEventBroadcaster != null)
            AnimationEventBroadcaster.AnimationEventTriggered += OnAnimationEvenTriggered;
    }

    protected virtual void Start()
    {
        if (Animator != null)
            Animator.enabled = true;
    }

    protected virtual void OnDisable()
    {
        if (AnimationEventBroadcaster != null)
            AnimationEventBroadcaster.AnimationEventTriggered -= OnAnimationEvenTriggered;
    }

    protected virtual void OnAnimationEvenTriggered(AnimationEvent eventArgs) { }
}