using System;
using UnityEngine;
using UnityEngine.InputSystem;

//This script is responsible for managing the player's input, such as movement, looking, jumping, sprinting, and interacting.
public class InputManager : MonoBehaviour
{ 
    public static InputManager Instance;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool interact;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [HideInInspector]
    public static event Action OnJumpGlobal;
    public static event Action OnSprintGlobal;
    public static event Action OnInteractGlobal;
    private bool blockInput = false;
    private bool blockPlayerMov = false;
    private bool blockPlayerRot = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        if (blockInput || blockPlayerMov)
            return;

        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (blockInput || blockPlayerRot)
            return;

        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        if (blockInput || blockPlayerMov)
            return;

        OnJumpGlobal?.Invoke();
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        if (blockInput || blockPlayerMov)
            return;

        OnSprintGlobal?.Invoke();
        SprintInput(value.isPressed);
    }

    public void OnInteract(InputValue value)
    {
        if (blockInput)
            return;

        OnInteractGlobal?.Invoke();
        InteractInput(value.isPressed);
    }
#endif

    #region PlayerUpdates
    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }
    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }
    #endregion

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    #region InputBlockManegment
    public void BlockInput()
    {
        blockInput = true;
    }
    public void BlockPlayerMoveAndRot()
    {
        blockPlayerMov = true;
        blockPlayerRot = true;
    }
    public void BlockPlayerMov()
    {
        blockPlayerMov = true;
    }
    public void BlockPlayerRot()
    {
        blockPlayerRot = true;
    }

    #endregion

    #region InputUnlockManegment
    public void UnblockInput()
    {
        blockInput = false;
    }
    public void UnblockPlayerMoveAndRot()
    {
        blockPlayerMov = false;
        blockPlayerRot = false;
    }
    public void UnblockPlayerMov()
    {
        blockPlayerMov = false;
    }
    public void UnblockPlayerRot()
    {
        blockPlayerRot = false;
    }

    #endregion
}