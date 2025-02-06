using System;
using System.Collections.Generic;
using UnityEngine;

public class PCAnimation : Entity
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [Space(5)]

    [SerializeField]
    private List<AnimationClip> _animationClips = new();

    [Header("Normalized Transition Durations")]
    [Tooltip("The duration of the transition, in normalized time, relative to the current state’s duration.")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _idleTransitionDuration = 0.02f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _walkTransitionDuration = 0.02f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _runTransitionDuration = 0.02f;

    private readonly Dictionary<int, string> _animationStates = new();
    private bool _isGrounded = true;
    private bool _isCurrentlyMoving = false;
    private bool _wasMovingBefore = false;
    private bool _isJumpTriggered = false;
    private bool _isSprintTriggered = false;

    public Dictionary<int, string> AnimationStates => _animationStates;

    private event Func<bool> TransitionCheck;

    protected override void Awake()
    {
        base.Awake();

        if (_animationStates.Count > 0)
            _animationStates.Clear();

        if (_animationClips.Count > 0)
            for (int i = 0; i < _animationClips.Count; i++)
                _animationStates.Add(Animator.StringToHash(_animationClips[i].name), _animationClips[i].name);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);

        _input.MoveInputHasChanged += OnMoveInputHasChanged;
        _input.SprintIsTriggered += OnSprintIsTriggered;
        _input.JumpIsTriggered += OnJumpIsTriggered;

        TransitionCheck += OnTransitionCheck;
    }

    protected override void Start()
    {
        base.Start();

        foreach (var animationState in _animationStates)
        {
            //Debug.LogFormat("AnimationState: {0} | Hash: {1}", animationState.Value, animationState.Key);

            if (!Animator.HasState(0, animationState.Key))
                Debug.LogErrorFormat("AnimationState {0} does not exist in {1} layer!", animationState.Value, Animator.GetLayerName(0));
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);

        _input.MoveInputHasChanged -= OnMoveInputHasChanged;
        _input.SprintIsTriggered -= OnSprintIsTriggered;
        _input.JumpIsTriggered -= OnJumpIsTriggered;

        TransitionCheck -= OnTransitionCheck;
    }

    protected virtual void OnValidate()
    {
        if (_input == null)
            _input = FindFirstObjectByType<SO_GameInputReader>();

        if (_animationStates.Count > 0)
            _animationStates.Clear();

        if (_animationClips.Count > 0)
            for (int i = 0; i < _animationClips.Count; i++)
                _animationStates.Add(Animator.StringToHash(_animationClips[i].name), _animationClips[i].name);
    }

    private bool OnTransitionCheck()
    {
        if (_isGrounded && !_isCurrentlyMoving)
            return SetAnimationState(0, "Idle", _idleTransitionDuration);

        if (_isGrounded && _isCurrentlyMoving && !_isSprintTriggered)
            return SetAnimationState(0, "Walk", _walkTransitionDuration);

        if (_isGrounded && _isCurrentlyMoving && _isSprintTriggered)
            return SetAnimationState(0, "Run", _runTransitionDuration);

        if (!_isGrounded)
            return SetAnimationState(0, "Jump", _idleTransitionDuration);

        return false;
    }

    private void OnGroundedStateChanged(object[] args)
    {
        if (args[0] is bool isGrounded && isGrounded != _isGrounded)
        {
            _isGrounded = isGrounded;
            TransitionCheck?.Invoke();
        }
    }

    private void OnJumpIsTriggered(bool isJumpTriggered)
    {
        if (isJumpTriggered != _isJumpTriggered)
        {
            _isJumpTriggered = isJumpTriggered;
            TransitionCheck?.Invoke();
        }
    }

    private void OnMoveInputHasChanged(Vector2 moveInput)
    {
        _isCurrentlyMoving = moveInput.sqrMagnitude >= 0.0001;

        if (_wasMovingBefore != _isCurrentlyMoving)
        {
            _wasMovingBefore = _isCurrentlyMoving;
            TransitionCheck?.Invoke();
        }
    }

    private void OnSprintIsTriggered(bool isSprintTriggered)
    {
        if (isSprintTriggered != _isSprintTriggered)
        {
            _isSprintTriggered = isSprintTriggered;
            TransitionCheck?.Invoke();
        }
    }

    public bool SetAnimationState(int layerIndex, string animationState, float normalizedTransitionDuration = 0.0f, float normalizedTimeOffset = 0.0f, float normalizedTransitionTime = 0.0f)
    {
        if (Animator.HasState(layerIndex, Animator.StringToHash(animationState)))
        {
            if (normalizedTransitionDuration == 0.0f)
                Animator.Play(animationState, layerIndex);
            else
                Animator.CrossFade(animationState, normalizedTransitionDuration, layerIndex, normalizedTimeOffset, normalizedTransitionTime);

            return true;
        }

        Debug.LogErrorFormat("AnimationState {0} does not exist in {1} layer!", animationState, Animator.GetLayerName(layerIndex));
        return false;
    }

    public bool SetAnimationState(string layerName, string animationState, float normalizedTransitionDuration = 0.0f, float normalizedTimeOffset = 0.0f, float normalizedTransitionTime = 0.0f)
    {
        int layerIndex = Animator.GetLayerIndex(layerName);

        if (layerIndex < 0)
        {
            Debug.LogErrorFormat("{0} layer does not exist in Animator!", layerName);
            return false;
        }

        if (Animator.HasState(layerIndex, Animator.StringToHash(animationState)))
        {
            if (normalizedTransitionDuration == 0.0f)
                Animator.Play(animationState, layerIndex);
            else
                Animator.CrossFade(animationState, normalizedTransitionDuration, layerIndex, normalizedTimeOffset, normalizedTransitionTime);

            return true;
        }

        Debug.LogErrorFormat("AnimationState {0} does not exist in {1} layer!", animationState, layerName);
        return false;
    }
}
