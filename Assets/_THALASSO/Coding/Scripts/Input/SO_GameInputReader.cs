using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(fileName = "NewGameInputReader", menuName = "Scriptable Objects/GameInputReader")]
public class SO_GameInputReader : ScriptableObject, GameInput.IPlayerActions, GameInput.IUIActions, GameInput.ICutsceneActions
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

    // Cutscene Actions
    public event Action SkipIsTriggered;

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
    private InputActionMap _previousActionMap;

    // Debug Member Values
    private bool _isMoveTriggered = false;
    private bool _isSprintTriggered = false;
    private Vector2 _moveInput = new();
    private Vector2 _lookInput = new();
    private bool _isJumpTriggered = false;
    private bool _isInteractTriggered = false;

    // Properties
    public ReadOnlyArray<InputControlScheme> ControlSchemes { get => _gameInput.controlSchemes; }
    public readonly Dictionary<int, InputActionMap> ActionMaps = new();
    public InputActionMap CurrentActionMap { get => _currentActionMap; }
    public InputActionMap PreviousActionMap { get => _previousActionMap; }
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
    public bool IsPauseActive { get; set; }
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

    #region Unity Lifecycle Methods
    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            _actionMaps = _gameInput.asset.actionMaps;

            _gameInput.Player.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
            _gameInput.Cutscene.SetCallbacks(this);

            _defaultActionMap = _actionMaps[0];
        }

        if (ActionMaps.Count > 0 && ActionMaps.Count != _actionMaps.Count)
        {
            ActionMaps.Clear();
            for (int i = 0; i < _actionMaps.Count; i++)
                ActionMaps.Add(i, _actionMaps[i]);
        }

        EnableDefaultActionMap();

        IsPauseActive = false;
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

        if (_currentActionMap.name == "UI")
            SetCursorSettings(true, CursorLockMode.Confined);
        else
            SetCursorSettings(false, CursorLockMode.Locked);
    }

    public bool SwitchCurrentActionMapTo(string actionMapName)
    {
        if (actionMapName == "UI")
            SetCursorSettings(true, CursorLockMode.Confined);
        else
            SetCursorSettings(false, CursorLockMode.Locked);

        if (actionMapName == _currentActionMap.name)
        {
            Debug.LogWarningFormat("Action map with name <color=yellow>'{0}'</color> in <color=cyan>'{1}'</color> is already active!", actionMapName, _gameInput.asset.name);
            return false;
        }

        _previousActionMap = _currentActionMap;

        foreach (var actionMap in _actionMaps)
        {
            if (actionMap.name == actionMapName && _currentActionMap.name != actionMapName)
            {
                _currentActionMap.Disable();
                actionMap.Enable();
                _currentActionMap = actionMap;
                Debug.Log(_currentActionMap);
                return true;
            }
        }

        Debug.LogErrorFormat("Cannot find action map with name <color=red>'{0}'</color> in <color=cyan>'{1}'</color>.", actionMapName, _gameInput.asset.name);
        return false;
    }

    //public void SwitchCurrentActionMap(Guid actionMapID)
    //{
    //    bool transitionSuccessful = false;

    //    foreach (var actionMap in _actionMaps)
    //    {
    //        if (actionMap.id == actionMapID && _currentActionMap.id != actionMapID)
    //        {
    //            _currentActionMap.Disable();
    //            actionMap.Enable();
    //            _currentActionMap = actionMap;
    //            transitionSuccessful = true;
    //            break;
    //        }
    //    }

    //    if (!transitionSuccessful)
    //        Debug.LogErrorFormat("Cannot find action map with Guid <color=red>'{0}'</color> in <color=cyan>'{1}'</color>.", actionMapID, _gameInput.asset.name);
    //}

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
        if (InteractIsTriggered is not null && context.phase == InputActionPhase.Performed)
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
        if (JumpIsTriggered is not null && context.performed)
            IsJumpTriggered = true;

        if (JumpIsTriggered is not null && context.canceled)
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
        {
            IsPauseActive = !IsPauseActive;
            PauseIsPerformed.Invoke();
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        if (SkipIsTriggered is not null && context.performed)
            SkipIsTriggered.Invoke();
    }
    #endregion

    #region UIActionMap CallbackFunctions
    public void OnNavigate(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }
    #endregion

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public bool SetCursorSettings(bool isCursorVisible, CursorLockMode cursorLockMode)
    {
        Cursor.visible = isCursorVisible;
        Cursor.lockState = cursorLockMode;
        return Cursor.visible == isCursorVisible && Cursor.lockState == cursorLockMode;
    }
}
