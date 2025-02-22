using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class AustinAnimationManager : MonoBehaviour
{
    public static event Action<AustinAnimationManager> SendReference;

    private SplineAnimate splineAnimator;
    private Animator animator;

    private GameObject austinBody;

    private void Start()
    {
        SendReference?.Invoke(this);
        splineAnimator = GetComponent<SplineAnimate>();
        animator = GetComponent<Animator>();
        austinBody = transform.GetChild(0).gameObject;
        austinBody.SetActive(false);
    }

    internal void AnimationWasTriggered(SplineContainer _splineContainer, float _playbackSpeed, float _startOffset)
    {
        splineAnimator.Container = _splineContainer;
        splineAnimator.MaxSpeed = _playbackSpeed;
        splineAnimator.StartOffset = _startOffset;

        splineAnimator.Restart(true);


        splineAnimator.Updated += OnAnimationUpdate;
        splineAnimator.Completed += OnAnimationCompleted;

        austinBody.SetActive(true);
    }

    Vector3 lastPosition = Vector3.zero;
    private void OnAnimationUpdate(Vector3 _pos, Quaternion _rot)
    {
        if(lastPosition == Vector3.zero)
            lastPosition = _pos;

        float curSpeed = Vector3.Distance(lastPosition, _pos) * 3;
        animator.SetFloat("SpeedMultipier", curSpeed);
        lastPosition = _pos;
    }

    private void OnAnimationCompleted()
    {
        austinBody.SetActive(false);
        splineAnimator.Completed -= OnAnimationCompleted;
    }
}
