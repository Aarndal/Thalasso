using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(fileName = "NewGameInputReader", menuName = "Scriptable Objects/GameInputReader")]
public class SO_GameInputReader : ScriptableObject, GameInput.IPlayerActions, GameInput.IUIActions
{
    // Player Actions
    public event Action<Vector2> MoveInputHasChanged;
    public event Action<Vector2, bool> LookInputHasChanged;
    public event Action<bool> JumpIsTriggered;
    public event Action<bool> SprintIsTriggered;
    public event Action<bool> InteractIsTriggered;
    public event Action PauseIsPerformed;

    // UI Actions
    // ...

    [SerializeField]
    private InputActionMap _defaultActionMap;

    [Header("Input Settings")]
    [SerializeField]
    private bool _invertXLookInput = false;
    [SerializeField]
    private bool _invertYLookInput = false;

    private GameInput _gameInput = default;
    private ReadOnlyArray<InputActionMap> _actionMaps;
    private InputActionMap _currentActionMap;

    // Debug Member Values
    private bool _isMoveTriggered;
    private bool _isSprintTriggered;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isJumpTriggered;
    private bool _isInteractTriggered;

    // Properties
    public ReadOnlyArray<InputControlScheme> ControlSchemes { get => _gameInput.controlSchemes; }
    public bool IsXLookInputInverted { get => _invertXLookInput; private set => _invertXLookInput = value; }
    public bool IsYLookInputInverted { get => _invertYLookInput; private set => _invertYLookInput = value; }
    public bool IsMoveTriggered
    {
        get => _isMoveTriggered;
        private set
        {
            if (_isMoveTriggered != value)
            {
                _isMoveTriggered = value;
            }
        }
    }
    public bool IsJumpTriggered
    {
        get => _isJumpTriggered;
        private set
        {
            if (_isJumpTriggered != value)
            {
                _isJumpTriggered = value;
                JumpIsTriggered?.Invoke(_isJumpTriggered);
            };
        }
    }
    public bool IsSprintTriggered
    {
        get => _isSprintTriggered;
        private set
        {
            if (_isSprintTriggered != value)
            {
                _isSprintTriggered = value;
                SprintIsTriggered?.Invoke(_isSprintTriggered);
            };
        }
    }
    public bool IsInteractTriggered
    {
        get => _isInteractTriggered;
        private set
        {
            if (_isInteractTriggered != value)
            {
                _isInteractTriggered = value;
                InteractIsTriggered?.Invoke(_isInteractTriggered);
            }
        }
    }

    #region Unity MonoBehaviour Methods
    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            _actionMaps = _gameInput.asset.actionMaps;

            _gameInput.Player.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            _defaultActionMap = _actionMaps[0];
        }

        EnableDefaultActionMap();
    }

    private void OnDisable()
    {
        DisableAllActionMaps();
    }
    #endregion

    #region Enable/Disable Action Maps
    public void EnableDefaultActionMap()
    {
        _defaultActionMap.Enable();
        _currentActionMap = _defaultActionMap;
    }

    public void SwitchCurrentActionMap(string actionMapName)
    {
        foreach (var actionMap in _actionMaps)
        {
            if (actionMap.name == actionMapName && _currentActionMap.name != actionMapName)
            {
                _currentActionMap.Disable();
                actionMap.Enable();
                _currentActionMap = actionMap;
                break;
            }
        }

        Debug.LogErrorFormat($"Cannot find action map '{actionMapName}' in '{_gameInput.asset.name}'.", this);
    }

    public void SwitchCurrentActionMap(Guid actionMapID)
    {
        foreach (var actionMap in _actionMaps)
        {
            if (actionMap.id == actionMapID && _currentActionMap.id != actionMapID)
            {
                _currentActionMap.Disable();
                actionMap.Enable();
                _currentActionMap = actionMap;
                break;
            }
        }
        Debug.LogErrorFormat($"Cannot find action map '{actionMapID}' in '{_gameInput.asset.name}'.", this);
    }

    public void DisableAllActionMaps()
    {
        foreach (var actionMap in _actionMaps)
            actionMap.Disable();
    }
    #endregion

    #region PlayerActionMap CallbackFunctions
    public void OnMove(InputAction.CallbackContext context)
    {
        if (MoveInputHasChanged is not null && context.phase == InputActionPhase.Performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            IsMoveTriggered = true;
            MoveInputHasChanged.Invoke(_moveInput);
        }

        if (MoveInputHasChanged is not null && context.phase == InputActionPhase.Canceled)
        {
            _moveInput = Vector2.zero;
            IsMoveTriggered = false;
            MoveInputHasChanged.Invoke(_moveInput);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (LookInputHasChanged is not null && context.phase == InputActionPhase.Performed)
        {
            _lookInput = context.ReadValue<Vector2>();
            LookInputHasChanged.Invoke(_lookInput, IsDeviceMouse(context));
        }

        if (LookInputHasChanged is not null && context.phase == InputActionPhase.Canceled)
        {
            _lookInput = Vector2.zero;
            LookInputHasChanged.Invoke(_lookInput, IsDeviceMouse(context));
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (InteractIsTriggered is not null && context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
            IsInteractTriggered = true;

        if (InteractIsTriggered is not null && context.phase == InputActionPhase.Canceled)
            IsInteractTriggered = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (JumpIsTriggered is not null && context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
            IsJumpTriggered = true;

        if (JumpIsTriggered is not null && context.phase == InputActionPhase.Canceled)
            IsJumpTriggered = false;

    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (SprintIsTriggered is not null && context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
            IsSprintTriggered = true;

        if (SprintIsTriggered is not null && context.phase == InputActionPhase.Canceled)
            IsSprintTriggered = false;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (PauseIsPerformed is not null && context.phase == InputActionPhase.Performed)
            PauseIsPerformed.Invoke();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region UIActionMap CallbackFunctions
    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
    #endregion

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";
}
