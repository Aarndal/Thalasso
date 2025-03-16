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
    private Image _dot = default;
    [SerializeField]
    private UIImageSwitch _imageSwitch = default;
    [SerializeField]
    private TextMeshProUGUI _text = default;

    [SerializeField, Tooltip("In Milliseconds")]
    private int _delayTime = 500;
    [SerializeField]
    private Color _activatableColor = Color.green;
    [SerializeField]
    private Color _nonActivatableColor = Color.red;

    private IAmInteractive _currentInteractiveObject = default;

    #region UnityLifecycleMethods
    private void Awake()
    {
        _imageSwitch = _imageSwitch != null ? _imageSwitch : GetComponentInChildren<UIImageSwitch>(true);
        _text = _text != null ? _text : GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void OnEnable()
    {
        if (_imageSwitch.gameObject.activeInHierarchy)
            _imageSwitch.gameObject.SetActive(false);

        GlobalEventBus.Register(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);

        _input.InteractIsTriggered += OnInteractIsTriggered;
        _input.ActionMapChanged += OnActionMapChanged;
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
        if (isTriggered)
        {
            if (_currentInteractiveObject == null)
                return;

            if (_currentInteractiveObject.IsActivatable)
                _imageSwitch.SwitchImageAndChangeColorForMilliseconds(_activatableColor, _delayTime);
            else
                _imageSwitch.SwitchImageAndChangeColorForMilliseconds(_nonActivatableColor, _delayTime);
        }
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is IAmInteractive interactiveObject)
            {
                if (!_imageSwitch.gameObject.activeInHierarchy)
                    _imageSwitch.gameObject.SetActive(true);

                _currentInteractiveObject = interactiveObject;
            }

            if (arg is null)
            {
                if (_imageSwitch.gameObject.activeInHierarchy)
                    _imageSwitch.gameObject.SetActive(false);

            }
        }
    }
}
