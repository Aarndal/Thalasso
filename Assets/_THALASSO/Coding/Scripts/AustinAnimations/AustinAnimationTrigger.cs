using System;
using UnityEngine;
using UnityEngine.Splines;

public class AustinAnimationTrigger : MonoBehaviour
{
    #region References
    private AustinAnimationManager manager;
    private void OnEnable()
    {
        AustinAnimationManager.SendReference += SaveManagerReference;
    }
    private void OnDisable()
    {
        AustinAnimationManager.SendReference -= SaveManagerReference;
    }
    private void SaveManagerReference(AustinAnimationManager _manager)
    {
        manager = _manager;
    }
    #endregion

    private SplineContainer splineContainer;

    [SerializeField] private float playbackSpeed;

    private void Start()
    {
        splineContainer = GetComponentInChildren<SplineContainer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && splineContainer != null)
        {
            manager.AnimationWasTriggered(splineContainer, playbackSpeed);
        }
    }
}
