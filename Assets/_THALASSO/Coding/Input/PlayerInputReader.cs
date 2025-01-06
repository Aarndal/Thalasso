using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewInputHandler", menuName = "Scriptable Objects/InputHandler")]
public class PlayerInputReader : ScriptableObject, GameInput.IPlayerActions
{
    public event Action<Vector2> MoveInputHasChanged;
    public event Action<Vector2> LookInputHasChanged;
    public event Action JumpIsPerformed;
    public event Action JumpIsCanceled;

    private Vector2 _moveInput;
    private bool _moveIsTriggered;
    private bool _sprintIsTriggered;
    private Vector2 _lookInput;
    private bool _jumpIsTriggered;
    private bool _interactIsTriggered;

    [Header("Input Settings")]
    [SerializeField]
    private bool _invertXLookInput = false;
    [SerializeField]
    private bool _invertYLookInput = false;

    private GameInput _gameInput;

    public bool MoveIsTriggered { get => _moveIsTriggered; private set => _moveIsTriggered = value; }
    public bool LookXInputIsInverted { get => _invertXLookInput; private set => _invertXLookInput = value; }
    public bool LookYInputIsInverted { get => _invertYLookInput; private set => _invertYLookInput = value; }
    public bool InteractIsTriggered { get => _interactIsTriggered; private set => _interactIsTriggered = value; }


    private void Awake()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            _gameInput.Player.SetCallbacks(this);
        }
    }

    private void OnEnable()
    {
        EnablePlayerInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    private void OnDestroy()
    {
        _gameInput.Player.RemoveCallbacks(this);
    }

    public void EnablePlayerInput()
    {
        _gameInput.Player.Enable();
        _gameInput.UI.Disable();
    }

    public void DisableAllInput()
    {
        _gameInput.Player.Disable();
        _gameInput.UI.Disable();
    }

    #region PlayerInput CallbackFunctions
    public void OnMove(InputAction.CallbackContext context)
    {
        if (MoveInputHasChanged != null && context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            MoveIsTriggered = true;
            MoveInputHasChanged?.Invoke(_moveInput);
        }

        if (MoveInputHasChanged != null && context.canceled)
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
        throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
    #endregion
}
