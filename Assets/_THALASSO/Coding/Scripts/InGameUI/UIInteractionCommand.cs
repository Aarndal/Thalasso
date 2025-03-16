using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Toggles the UI cue that an object can be interacted with, and gives a feedback to the player if the interaction was successful or not.
public class UIInteractionCommand : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;
    [SerializeField]
    private GameObject _interactionHint = default;

    [SerializeField, Tooltip("In Seconds")]
    private float _delayTime = 1.0f;
    [SerializeField]
    private Color _activatableColor = Color.green;
    [SerializeField]
    private Color _nonActivatableColor = Color.red;

    private Image _dot = default;
    private IAmInteractive _currentInteractiveObject = default;

    private UIImageSpriteLooper _imageSwitch = default;
    private TextMeshProUGUI _text = default;

    #region UnityLifecycleMethods
    private void Awake()
    {
        _dot = _dot != null ? null : GetComponentInParent<Image>(true);
        _imageSwitch = _imageSwitch != null ? _imageSwitch : GetComponentInChildren<UIImageSpriteLooper>(true);
        _text = _text != null ? _text : GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void OnEnable()
    {
        if (_interactionHint.activeInHierarchy)
            _interactionHint.SetActive(false);

        _input.ActionMapChanged += OnActionMapChanged;
        _input.InteractIsTriggered += OnInteractIsTriggered;

        GlobalEventBus.Register(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);

        _input.InteractIsTriggered -= OnInteractIsTriggered;
        _input.ActionMapChanged -= OnActionMapChanged;
    }
    #endregion

    private void OnActionMapChanged(InputActionMap previousMap, InputActionMap currentMap)
    {
        if (currentMap.name != "Player")
            _dot.enabled = false;
        else
            _dot.enabled = true;
    }

    private void OnInteractIsTriggered(bool isTriggered)
    {
        if (isTriggered && _currentInteractiveObject != null)
        {
            if (_currentInteractiveObject.IsActivatable)
                _imageSwitch.StartLoop(_delayTime, true, _activatableColor);
            else
                _imageSwitch.StartLoop(_delayTime, true, _nonActivatableColor);
        }
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        if (args[1] is IAmInteractive interactiveObject)
        {
            if (!_interactionHint.activeInHierarchy || !_imageSwitch.enabled)
            {
                _interactionHint.SetActive(true);
                _imageSwitch.enabled = true;
            }

            _currentInteractiveObject = interactiveObject;

            return;
        }

        if (args[1] is null)
        {
            if (_interactionHint.activeInHierarchy)
            {
                _interactionHint.SetActive(false);
            }

            //_imageSwitch.StopLoop();

            return;
        }
    }
}
