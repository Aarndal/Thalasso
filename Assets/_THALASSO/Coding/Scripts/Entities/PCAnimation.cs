using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
public class PCAnimation : Entity
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [Header("Normalized Transition Durations")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _idleTransitionTime = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _walkTransitionTime = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _runTransitionTime = 0.0f;

    private bool _isGrounded = true;
    private bool _isCurrentlyMoving = false;
    private bool _wasMovingBefore = false;
    private bool _isJumpTriggered = false;
    private bool _isSprintTriggered = false;

    private event Action ConditionChanged;

    protected override void OnEnable()
    {
        base.OnEnable();

        GlobalEventBus.Register(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);

        _input.MoveInputHasChanged += OnMoveInputHasChanged;
        _input.SprintIsTriggered += OnSprintIsTriggered;
        _input.JumpIsTriggered += OnJumpIsTriggered;

        ConditionChanged += OnConditionChanged;
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        GlobalEventBus.Deregister(GlobalEvents.Player.GroundedStateChanged, OnGroundedStateChanged);

        _input.MoveInputHasChanged -= OnMoveInputHasChanged;
        _input.SprintIsTriggered -= OnSprintIsTriggered;
        _input.JumpIsTriggered -= OnJumpIsTriggered;

        ConditionChanged -= OnConditionChanged;
    }

    protected virtual void OnValidate()
    {
        if (_input == null)
            _input = FindFirstObjectByType<SO_GameInputReader>();
    }

    protected override void OnAnimationEvenTriggered(AnimationEvent args)
    {
        if (args.stringParameter == "Walk")
        {

        }
    }

    private void OnConditionChanged()
    {
        if ((!_isGrounded && _isCurrentlyMoving || !_isCurrentlyMoving) && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            SetAnimationState(0, "Idle", _idleTransitionTime);

        if (_isGrounded && _isCurrentlyMoving && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            SetAnimationState(0, "Walk", _walkTransitionTime);

        if (_isGrounded && _isCurrentlyMoving && _isSprintTriggered && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            SetAnimationState(0, "Run", _runTransitionTime);
    }

    private void OnGroundedStateChanged(object[] args)
    {
        if (args[0] is bool isGrounded && isGrounded != _isGrounded)
        {
            _isGrounded = isGrounded;
            ConditionChanged?.Invoke();
        }
    }

    private void OnJumpIsTriggered(bool isJumpTriggered)
    {
        if (isJumpTriggered != _isJumpTriggered)
        {
            _isJumpTriggered = isJumpTriggered;
            ConditionChanged?.Invoke();
        }
    }

    private void OnMoveInputHasChanged(Vector2 moveInput)
    {
        _isCurrentlyMoving = moveInput.sqrMagnitude >= 0.001;

        if (_wasMovingBefore != _isCurrentlyMoving)
        {
            _wasMovingBefore = _isCurrentlyMoving;
            ConditionChanged?.Invoke();
        }
    }

    private void OnSprintIsTriggered(bool isSprintTriggered)
    {
        if (isSprintTriggered != _isSprintTriggered)
        {
            _isSprintTriggered = isSprintTriggered;
            ConditionChanged?.Invoke();
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
