using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance;

    [Header("Character Input Values")]
    [SerializeField]
    private Vector2 _moveInput;
    [SerializeField] 
    private bool _moveIsTriggered;
    [SerializeField] 
    private bool _sprintIsTriggered;
    [Space(5)]
    [SerializeField] 
    private Vector2 _lookInput;
    [SerializeField] 
    private bool _jumpIsTriggered;
    [SerializeField] 
    private bool _interactIsTriggered;

    [Space(5)]

    [Header("Input Settings")]
    [SerializeField] 
    private bool _invertXLookInput = false;
    [SerializeField] 
    private bool _invertYLookInput = false;
    
    private InputSystem_Actions _playerInputActions;
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;

    //Properties
    public PlayerInput PlayerInput { get; private set; }
    public Vector2 MoveInput { get => _moveInput; private set => _moveInput = value; }
    public bool MoveIsTriggered { get => _moveIsTriggered; private set => _moveIsTriggered = value; }
    public bool SprintIsTriggered { get => _sprintIsTriggered; private set => _sprintIsTriggered = value; }
    public Vector2 LookInput { get => _lookInput; private set => _lookInput = value; }
    public bool LookXInputIsInverted { get => _invertXLookInput; private set => _invertXLookInput = value; }
    public bool LookYInputIsInverted { get => _invertYLookInput; private set => _invertYLookInput = value; }
    public bool JumpIsTriggered { get => _jumpIsTriggered; private set => _jumpIsTriggered = value; }
    public bool InteractIsTriggered { get => _interactIsTriggered; private set => _interactIsTriggered = value; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        PlayerInput = GetComponent<PlayerInput>();
        
        _playerInputActions = new InputSystem_Actions();

        _playerActionMap = _playerInputActions.Player;
        _uiActionMap = _playerInputActions.UI;

        _moveAction = _playerInputActions.Player.Move;
        _lookAction = _playerInputActions.Player.Look;
        _jumpAction = _playerInputActions.Player.Jump;
        _sprintAction = _playerInputActions.Player.Sprint;
        _interactAction = _playerInputActions.Player.Interact;
    }

    private void OnEnable()
    {
        _playerActionMap.Enable();
        RegisterInputActions();
    }

    private void OnDisable()
    {
        DeregisterInputActions();
        _playerActionMap.Disable();
    }

    private void RegisterInputActions()
    {
        _moveAction.performed += OnMoveInput;
        _moveAction.canceled += OnMoveInput;
        
        _sprintAction.performed += OnSprintInput;
        _sprintAction.canceled += OnSprintInput;

        _lookAction.performed += OnLookInput;
        _lookAction.canceled += OnLookInput;

        _jumpAction.started += OnJumpInput;
        _jumpAction.canceled += OnJumpInput;

        _interactAction.performed += OnInteractInput;
        _interactAction.canceled += OnInteractInput;
    }

    private void DeregisterInputActions()
    {
        _moveAction.performed -= OnMoveInput;
        _moveAction.canceled -= OnMoveInput;
        
        _sprintAction.performed -= OnSprintInput;
        _sprintAction.canceled -= OnSprintInput;

        _lookAction.performed -= OnLookInput;
        _lookAction.canceled -= OnLookInput;

        _jumpAction.started -= OnJumpInput;
        _jumpAction.canceled -= OnJumpInput;

        _interactAction.performed -= OnInteractInput;
        _interactAction.canceled -= OnInteractInput;
    }

    #region Input Methods
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        MoveIsTriggered = !(MoveInput == Vector2.zero);
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        JumpIsTriggered = context.ReadValueAsButton();
    }

    private void OnSprintInput(InputAction.CallbackContext context)
    {
        SprintIsTriggered = context.ReadValueAsButton();
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        InteractIsTriggered = context.ReadValueAsButton();
    }
    #endregion
}
