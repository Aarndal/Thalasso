using UnityEngine;

public class PCInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [SerializeField]
    private TargetProvider _targetProvider = default;

    private IAmOperable _currentTarget = null;

    private void Awake()
    {
        if (_input == null)
            Debug.LogError("Input is not set in PCInteraction script.");
        //throw new ArgumentNullException(nameof(_input), "Input is not set in PCInteraction script.");

        if (_targetProvider == null)
            _targetProvider = GetComponentInChildren<OperableObjectTargetProvider>();

        if (_targetProvider == null)
            Debug.LogError("TargetProvider is not set in PCInteraction script.");
        //throw new ArgumentNullException(nameof(_targetProvider), "TargetProvider is not set in PCInteraction script.");
    }

    private void OnEnable()
    {
        _input.InteractIsTriggered += OnInteractIsTriggered;
        _targetProvider.TargetChanged += OnTargetChanged;
    }

    private void OnDisable()
    {
        _input.InteractIsTriggered -= OnInteractIsTriggered;
        _targetProvider.TargetChanged -= OnTargetChanged;
    }

    private void OnInteractIsTriggered(bool isInteractTriggered)
    {
        if (isInteractTriggered && _targetProvider.HasTarget)
            _currentTarget.Operate(transform);
    }

    private void OnTargetChanged(Transform oldTarget, Transform newTarget)
    {
        if(newTarget == null)
            _currentTarget = null;

        if (newTarget != null)
            _currentTarget = newTarget.GetComponent<IAmOperable>();
    }

}
