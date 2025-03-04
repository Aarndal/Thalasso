using System;
using System.Collections;
using System.Threading.Tasks;
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

    [SerializeField, Tooltip("In Milliseconds")]
    private int _delayTime = 500;
    [SerializeField]
    private Color _defaultColor = Color.white;
    [SerializeField]
    private Color _activatableColor = Color.green;
    [SerializeField]
    private Color _nonActivatableColor = Color.red;

    private IAmInteractive _currentInteractiveObject = default;
    private bool _isChangingColor = false;

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

        _input.InteractIsTriggered += OnInteractIsTriggered;
        _input.ActionMapChanged += OnActionMapChanged;
    }

    private void Reset()
    {
        _text = _text != null ? _text : GetComponentInChildren<TextMeshProUGUI>();

        _text.color = _defaultColor;
    }

    private void Start()
    {
        _text.color = _defaultColor;
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);

        _input.InteractIsTriggered -= OnInteractIsTriggered;
        _input.ActionMapChanged -= OnActionMapChanged;
    }
    #endregion

    private async void ChangeColorForMilliseconds(UnityEngine.UI.Graphic graphic, Color newColor)
    {
        if (_isChangingColor)
            return;

        _isChangingColor = true;
        graphic.color = newColor;

        await Task.Delay(_delayTime);

        graphic.color = _defaultColor;
        _isChangingColor = false;
    }

    private void OnActionMapChanged(InputActionMap previousMap, InputActionMap currentMap)
    {
        if (currentMap.name != "Player")
            _dot.SetActive(false);
        else
            _dot.SetActive(true);
    }

    private void OnInteractIsTriggered(bool isTriggered)
    {
        if (isTriggered)
            if (_currentInteractiveObject.IsActivatable)
                ChangeColorForMilliseconds(_text, _activatableColor);
            else
                ChangeColorForMilliseconds(_text, _nonActivatableColor);
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is IAmInteractive interactiveObject)
            {
                if (!_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(true);

                _currentInteractiveObject = interactiveObject;
            }

            if (arg is null)
            {
                if (_interactionButton.activeInHierarchy)
                    _interactionButton.SetActive(false);
            }
        }
    }
}
