using ProgressionTracking;
using UnityEngine;
using UnityEngine.Splines;

public class AustinAnimationTrigger : MonoBehaviour
{
    #region References
    private AustinAnimationManager manager;
    private void Awake()
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
    private Collider triggerCollider;

    [SerializeField] private float playbackSpeed = 7;
    [SerializeField] private float startOffset = 0;
    [SerializeField] private bool isProgressDepended = false;
    [SerializeField] private SO_SolvableData progressData;

    private void Start()
    {
        splineContainer = GetComponentInChildren<SplineContainer>();
        triggerCollider = GetComponent<Collider>();
        triggerCollider.enabled = true;

        if (isProgressDepended)
        {
            triggerCollider.enabled = false;
            progressData.ValueChanged += OnProgressChanged;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && splineContainer != null)
        {
            manager.AnimationWasTriggered(splineContainer, playbackSpeed, startOffset);
            triggerCollider.enabled = false;
            if (isProgressDepended)
            {
                progressData.ValueChanged -= OnProgressChanged;
            }
        }
    }
    private void OnProgressChanged(uint _id, bool _isSolved)
    {
        triggerCollider.enabled = true;
    }

}
