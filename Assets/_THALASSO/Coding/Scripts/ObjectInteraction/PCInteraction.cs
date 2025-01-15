using System;
using UnityEngine;

public class PCInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SO_GameInputReader _input = default;

    [SerializeField]
    private TargetProvider _targetProvider = default;

    private IAmInteractive _currentTarget = null;

    private void Awake()
    {
        if (_input == null)
            Debug.LogError("Input is not set in PCInteraction script.");
        //throw new ArgumentNullException(nameof(_input), "Input is not set in PCInteraction script.");

        if (_targetProvider == null)
            _targetProvider = GetComponentInChildren<InteractiveObjectTargetProvider>();

        if (_targetProvider == null)
            Debug.LogError("TargetProvider is not set in PCInteraction script.");
        //throw new ArgumentNullException(nameof(_targetProvider), "TargetProvider is not set in PCInteraction script.");

        _currentTarget = null;
    }

    private void OnEnable()
    {
        _input.InteractIsTriggered += OnInteractIsTriggered;
        _targetProvider.NewTargetDetected += OnNewTargetDetected;
        _targetProvider.TargetLost += (o) => _currentTarget = null;
    }

    private void OnDisable()
    {
        _targetProvider.TargetLost -= (o) => _currentTarget = null;
        _targetProvider.NewTargetDetected -= OnNewTargetDetected;
        _input.InteractIsTriggered -= OnInteractIsTriggered;
    }

    private void OnInteractIsTriggered(bool isInteractTriggered)
    {
        if (isInteractTriggered && _targetProvider.HasTarget)
            _currentTarget.Interact(this.transform);
    }

    private void OnNewTargetDetected(Transform newTarget)
    {
        _currentTarget = newTarget.GetComponent<IAmInteractive>();
    }

}
