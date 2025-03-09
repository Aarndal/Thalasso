using System;
using UnityEngine;
using UnityEngine.Splines;

public class AustinAnimationManager : MonoBehaviour
{
    public static event Action<AustinAnimationManager> SendReference;

    private SplineAnimate splineAnimator;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        splineAnimator = animator.gameObject.GetComponent<SplineAnimate>();
    }

    private void OnEnable()
    {
        splineAnimator.Updated += OnSplineAnimationUpdated;
        splineAnimator.Completed += OnSplineAnimationCompleted;

        animator.gameObject.SetActive(false);
    }

    private void Start()
    {
        SendReference?.Invoke(this);
    }

    private void OnDisable()
    {
        splineAnimator.Updated -= OnSplineAnimationUpdated;
        splineAnimator.Completed -= OnSplineAnimationCompleted;
    }

    internal void AnimationWasTriggered(SplineContainer _splineContainer, float _playbackSpeed, float _startOffset)
    {
        animator.gameObject.SetActive(true);
        
        splineAnimator.Container = _splineContainer;
        splineAnimator.MaxSpeed = _playbackSpeed;
        splineAnimator.StartOffset = _startOffset;

        splineAnimator.Restart(true);
    }

    Vector3 lastPosition = Vector3.zero;
    private void OnSplineAnimationUpdated(Vector3 _pos, Quaternion _rot)
    {
        if (lastPosition == Vector3.zero)
            lastPosition = _pos;

        float curSpeed = Vector3.Distance(lastPosition, _pos) * 3;
        animator.SetFloat("SpeedMultipier", curSpeed);
        lastPosition = _pos;
    }

    private void OnSplineAnimationCompleted()
    {
        animator.gameObject.SetActive(false);
    }
}
