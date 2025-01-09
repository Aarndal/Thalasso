using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(fileName = "NewInputHandler", menuName = "Scriptable Objects/InputHandler")]
public class PlayerInputReader : ScriptableObject, GameInput.IPlayerActions, GameInput.IUIActions
{
    // Player Actions
    public event Action<Vector2> MoveInputHasChanged;
    public event Action<Vector2> LookInputHasChanged;
    public event Action JumpIsPerformed;
    public event Action JumpIsCanceled;
    public event Action<bool> SprintIsTriggered;

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
    private Vector2 _moveInput;
    private bool _moveIsTriggered;
    private bool _sprintIsTriggered;
    private Vector2 _lookInput;
    private bool _jumpIsTriggered;
    private bool _interactIsTriggered;

    // Properties
    public ReadOnlyArray<InputControlScheme> ControlSchemes { get => _gameInput.controlSchemes; }
    public bool MoveIsTriggered { get => _moveIsTriggered; private set => _moveIsTriggered = value; }
    public bool LookXInputIsInverted { get => _invertXLookInput; private set => _invertXLookInput = value; }
    public bool LookYInputIsInverted { get => _invertYLookInput; private set => _invertYLookInput = value; }
    public bool InteractIsTriggered { get => _interactIsTriggered; private set => _interactIsTriggered = value; }


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

    public void DisableAllActionMaps()
    {
        foreach (var actionMap in _actionMaps)
            actionMap.Disable();
    }

    #region PlayerActionMap CallbackFunctions
    public void OnMove(InputAction.CallbackContext context)
    {
        if (MoveInputHasChanged != null && context.phase == InputActionPhase.Performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            MoveIsTriggered = true;
            MoveInputHasChanged?.Invoke(_moveInput);
        }

        if (MoveInputHasChanged != null && context.phase == InputActionPhase.Canceled)
        {
            _moveInput = Vector2.zero;
            MoveIsTriggered = false;
            MoveInputHasChanged?.Invoke(_moveInput);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (LookInputHasChanged != null && context.performed)
        {
            _lookInput = context.ReadValue<Vector2>();
            LookInputHasChanged?.Invoke(_lookInput);
        }

        if (LookInputHasChanged != null && context.canceled)
        {
            _lookInput = Vector2.zero;
            LookInputHasChanged?.Invoke(_lookInput);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (JumpIsPerformed != null && context.performed)
        {
            JumpIsPerformed?.Invoke();
            _jumpIsTriggered = true;
        }

        if (JumpIsCanceled != null && context.canceled)
        {
            JumpIsCanceled?.Invoke();
            _jumpIsTriggered = false;
        }
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
        if (SprintIsTriggered != null && context.performed)
        {
            SprintIsTriggered?.Invoke(true);
            _sprintIsTriggered = true;
        }

        if (SprintIsTriggered != null && context.canceled)
        {
            SprintIsTriggered?.Invoke(false);
            _sprintIsTriggered = false;
        }
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
}
