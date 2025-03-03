using System;
using UnityEngine;

public class UIInteractionCommand : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactionButton = default;

    private void Start()
    {
        if (_interactionButton.activeInHierarchy)
            _interactionButton.SetActive(false);
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is Transform newTarget)
            {
                if (!_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(true);
            }

            if (arg is null)
            {
                if (_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(false);
            }
        }
    }
}
