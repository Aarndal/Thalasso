using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class AustinAnimationManager : MonoBehaviour
{
    public static event Action<AustinAnimationManager> SendReference;

    private SplineAnimate splineAnimator;

    private GameObject austinBody;

    private void OnEnable()
    { 
    }
    private void Start()
    {
        SendReference?.Invoke(this);
        splineAnimator = GetComponent<SplineAnimate>();
        austinBody = transform.GetChild(0).gameObject;
    }

    internal void AnimationWasTriggered(SplineContainer splineContainer, float playbackSpeed)
    {
        splineAnimator.Container = splineContainer;
        splineAnimator.MaxSpeed = playbackSpeed;

        splineAnimator.Restart(true);


        splineAnimator.Completed += OnAnimationCompleted;
        austinBody.SetActive(true);
    }

    private void OnAnimationCompleted()
    {
        austinBody.SetActive(true);
        splineAnimator.Completed -= OnAnimationCompleted;
    }
}
