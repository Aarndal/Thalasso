using Eflatun.SceneReference;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewGameInputReader", menuName = "Scriptable Objects/GameInputReader")]
public class SO_GameInputReader : ScriptableObject, GameInput.IPlayerActions, GameInput.IUIActions, GameInput.ICutsceneActions
{
    public event Action<InputActionMap, InputActionMap> ActionMapChanged;

    // Player Actions
    public event Action<Vector2> MoveInputHasChanged;
    public event Action<Vector2, bool> LookInputHasChanged;
    public event Action<bool> JumpIsTriggered;
    public event Action<bool> SprintIsTriggered;
    public event Action<bool> InteractIsTriggered;

    // UI Actions
    // ...

    // Cutscene Actions
    public event Action SkipIsPerformed;

    [SerializeField]
    private InputActionMap _defaultActionMap;

    [SerializeField]
    private SceneReference _pauseMenu;

    [Header("Input Settings")]
    [SerializeField]
    private bool _invertXLookInput = false;
    [SerializeField]
    private bool _invertYLookInput = false;

    private GameInput _gameInput = default;
    private ReadOnlyArray<InputActionMap> _actionMaps;
    private InputActionMap _currentActionMap;
    private InputActionMap _previousActionMap;
    private InputDevice _currentInputDevice;

    // Debug Member Values
    private bool _isMoveTriggered = false;
    private bool _isSprintTriggered = false;
    private Vector2 _moveInput = new();
    private Vector2 _lookInput = new();
    private bool _isJumpTriggered = false;
    private bool _isInteractTriggered = false;
    private bool _isPauseActive = false;

    #region Properties
    public readonly Dictionary<string, InputActionMap> ActionMaps = new();
    public InputDevice CurrentInputDevice { get => _currentInputDevice; }
    public InputActionMap CurrentActionMap
    {
        get => _currentActionMap;
        private set
        {
            if (value != _currentActionMap)
            {
                if (_currentActionMap != null)
                    _previousActionMap = _currentActionMap;

                _currentActionMap = value;

                ActionMapChanged?.Invoke(_previousActionMap, _currentActionMap);

                if (_currentActionMap.name == "UI")
                    SetCursorSettings(true, CursorLockMode.Confined);
                else
                    SetCursorSettings(false, CursorLockMode.Locked);
            }
        }
    }
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
            }
        }
    }
    public bool IsPauseActive
    {
        get => _isPauseActive;
        private set
        {
            if (value != _isPauseActive)
            {
                _isPauseActive = value;
            }
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
            }
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
    #endregion

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
        
        if (ActionMaps.Count > 0 || ActionMaps.Count != _actionMaps.Count)
        {
            ActionMaps.Clear();
            for (int i = 0; i < _actionMaps.Count; i++)
                ActionMaps.Add(_actionMaps[i].name, _actionMaps[i]);
        }
        
        EnableDefaultActionMap();

        IsPauseActive = false;

        GlobalEventBus.Register(GlobalEvents.UI.MenuOpened, OnMenuOpened);
        GlobalEventBus.Register(GlobalEvents.UI.MenuClosed, OnMenuClosed);
        GlobalEventBus.Register(GlobalEvents.Game.IsPaused, OnGameIsPaused);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Game.IsPaused, OnGameIsPaused);
        GlobalEventBus.Deregister(GlobalEvents.UI.MenuClosed, OnMenuClosed);
        GlobalEventBus.Deregister(GlobalEvents.UI.MenuOpened, OnMenuOpened);

        DisableAllActionMaps();
    }
    #endregion

    #region Enable/Disable Action Maps
    public void EnableDefaultActionMap()
    {
        _defaultActionMap.Enable();
        CurrentActionMap = _defaultActionMap;
        _previousActionMap = CurrentActionMap;

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            SwitchCurrentActionMap("UI");
    }

    public bool SwitchCurrentActionMap(string newActionMapName)
    {
        if (!ActionMaps.TryGetValue(newActionMapName, out InputActionMap newActionMap))
        {
            Debug.LogErrorFormat("Cannot find action map with name <color=red>'{0}'</color> in <color=cyan>'{1}'</color>.", newActionMapName, _gameInput.asset.name);
            return false;
        }

        if (newActionMapName == CurrentActionMap.name)
        {
            _previousActionMap = CurrentActionMap;
            return false;
        }

        CurrentActionMap.Disable();
        newActionMap.Enable();
        CurrentActionMap = newActionMap;
        return true;
    }

    public void DisableAllActionMaps()
    {
        ActionMaps.Values.ToList().ForEach(actionMap => actionMap.Disable());
    }
    #endregion

    #region PlayerActionMap CallbackFunctions
    public void OnMove(InputAction.CallbackContext context)
    {
        _currentInputDevice = GetCurrentInputDevice(context);

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
        _currentInputDevice = GetCurrentInputDevice(context);

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
        //_currentInputDevice = GetCurrentInputDevice(context);

        if (context.phase == InputActionPhase.Performed && _pauseMenu.LoadedScene.isLoaded)
        {
            IsPauseActive = !IsPauseActive;
            GlobalEventBus.Raise(GlobalEvents.Game.IsPaused, IsPauseActive);
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
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

    #region CutsceneActionMap CallbackFunctions
    public void OnSkip(InputAction.CallbackContext context)
    {
        if (SkipIsPerformed is not null && context.performed)
            SkipIsPerformed.Invoke();
    }
    #endregion

    public bool SetCursorSettings(bool isCursorVisible, CursorLockMode cursorLockMode)
    {
        Cursor.visible = isCursorVisible;
        Cursor.lockState = cursorLockMode;
        return Cursor.visible == isCursorVisible && Cursor.lockState == cursorLockMode;
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => GetCurrentInputDevice(context) == Mouse.current;

    private InputDevice GetCurrentInputDevice(InputAction.CallbackContext context)
    {
        return context.control.device;
    }

    private InputControl GetInputControl(InputAction.CallbackContext context)
    {
        return context.control;
    }

    private void OnGameIsPaused(object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is bool paused && !paused)
            {
                IsPauseActive = paused;
                break;
            }
        }
    }

    private void OnMenuOpened(object[] args)
    {
        SwitchCurrentActionMap("UI");
    }

    private void OnMenuClosed(object[] args)
    {
        SwitchCurrentActionMap(PreviousActionMap.name);
    }
}
