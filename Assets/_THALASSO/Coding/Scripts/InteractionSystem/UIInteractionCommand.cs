using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInteractionCommand : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;
    [SerializeField]
    private GameObject _dot = default;
    [SerializeField]
    private GameObject _interactionButton = default;
    [SerializeField]
    private TextMeshProUGUI _text = default;

    [SerializeField]
    private Color _activatableColor = Color.green;
    [SerializeField]
    private Color _nonActivatableColor = Color.red;

    #region UnityLifecycleMethods
    private void Awake()
    {
        if (!_interactionButton.activeInHierarchy)
            _interactionButton.SetActive(true);

        _text = _text != null ? _text : GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (_interactionButton.activeInHierarchy)
            _interactionButton.SetActive(false);

        GlobalEventBus.Register(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);
        _input.ActionMapChanged += OnActionMapChanged;
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);
        _input.ActionMapChanged -= OnActionMapChanged;
    }
    #endregion

    private void OnActionMapChanged(InputActionMap previousMap, InputActionMap currentMap)
    {
        if (currentMap.name != "Player")
            _dot.SetActive(false);
        else
            _dot.SetActive(true);
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is Transform)
            {
                if (!_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(true);
            }

            if (arg is IAmInteractive interactiveObject)
            {
                if (interactiveObject.IsActivatable)
                    _text.color = _activatableColor;
                else
                    _text.color = _nonActivatableColor;
            }

            if (arg is null)
            {
                if (_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(false);
            }
        }
    }
}
