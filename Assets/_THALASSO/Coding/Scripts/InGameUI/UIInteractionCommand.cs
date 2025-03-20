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

    private UIImageSpriteLooper _spriteLooper = default;

    #region UnityLifecycleMethods
    private void Awake()
    {
        _dot = _dot != null ? _dot : GetComponentInParent<Image>(true);
        _spriteLooper = _spriteLooper != null ? _spriteLooper : GetComponentInChildren<UIImageSpriteLooper>(true);
    }

    private void OnEnable()
    {
        if (_interactionHint.activeInHierarchy)
            _interactionHint.SetActive(false);

        _input.ActionMapChanged += OnActionMapChanged;
        _input.InteractIsTriggered += OnInteractIsTriggered;

        GlobalEventBus.Register(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);

        _spriteLooper.ReachedEndOfArray += OnReachedEndOfArray;
        _spriteLooper.ReachedStartOfArray += OnReachedStartOfArray;
    }

    private void OnDisable()
    {
        _spriteLooper.ReachedStartOfArray -= OnReachedStartOfArray;
        _spriteLooper.ReachedEndOfArray -= OnReachedEndOfArray;

        GlobalEventBus.Deregister(GlobalEvents.Player.InteractiveTargetChanged, OnInteractiveTargetChanged);

        _input.InteractIsTriggered -= OnInteractIsTriggered;
        _input.ActionMapChanged -= OnActionMapChanged;
    }
    #endregion

    private void OnActionMapChanged(InputActionMap previousMap, InputActionMap currentMap)
    {
        if (currentMap.name != "Player")
        {
            _interactionHint.SetActive(false);
            _dot.enabled = false;
            return;
        }

        if(_currentInteractiveObject != null)
            _interactionHint.SetActive(true);
        else
            _interactionHint.SetActive(false);

        _dot.enabled = true;
    }

    private void OnInteractIsTriggered(bool isTriggered)
    {
        if (isTriggered && _currentInteractiveObject != null)
        {
            _spriteLooper.StartLoop(_delayTime, 1);
        }
    }

    private void OnInteractiveTargetChanged(object[] args)
    {
        if (args[1] is IAmInteractive interactiveObject)
        {
            if (!_interactionHint.activeInHierarchy || !_spriteLooper.enabled)
            {
                _interactionHint.SetActive(true);
                _spriteLooper.enabled = true;
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

            _currentInteractiveObject = null; //Change necessary?

            _spriteLooper.StopLoop();

            return;
        }
    }

    private void OnReachedStartOfArray()
    {
        if (_spriteLooper.CurrentColor != _spriteLooper.DefaultColor)
            _spriteLooper.SetColor(_spriteLooper.DefaultColor);
    }

    private void OnReachedEndOfArray()
    {
        if (_currentInteractiveObject == null)
            return;

        if (_currentInteractiveObject.IsActivatable)
            _spriteLooper.SetColor(_activatableColor);
        else
            _spriteLooper.SetColor(_nonActivatableColor);
    }

}
