using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    public static InputAction MOVE;
    public static InputAction INTERACT;
    public static InputAction LOOK;
    public static InputAction ATTACK;
    public static InputAction RELOAD;
    public static InputAction HOTKEY;
    public static InputAction SPRINT;

    public static event Action<Vector2> MoveEvent;
    public static event Action InteractEvent;
    public static event Action<Vector2> LookEvent;
    public static event Action AttackEvent;

    public static event Action ReloadEvent;
    public static event Action ReloadCancelledEvent;

    public static event Action<Vector2> HotkeyEvent;

    public static event Action SprintEvent;
    public static event Action SprintCancelledEvent;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        MOVE = _inputActions.Player.Move;
        MOVE.Enable();
        MOVE.performed += OnMovePerformed;

        INTERACT = _inputActions.Player.Interact;
        INTERACT.Enable();
        INTERACT.performed += OnInteractPerformed;

        LOOK = _inputActions.Player.Look;
        LOOK.Enable();
        LOOK.performed += OnLookPerformed;

        ATTACK = _inputActions.Player.Attack;
        ATTACK.Enable();
        ATTACK.performed += OnAttackPerformed;

        RELOAD = _inputActions.Player.Reload;
        RELOAD.Enable();
        RELOAD.performed += OnReload;
        RELOAD.canceled += OnReload;

        SPRINT = _inputActions.Player.Sprint;
        SPRINT.Enable();
        SPRINT.performed += OnSprint;
        SPRINT.canceled += OnSprint;

        HOTKEY = _inputActions.Player.Hotkey;
        HOTKEY.Enable();
        HOTKEY.performed += OnHotkeyPerformed;
    }

    private void OnDisable()
    {
        MOVE.Disable();
        INTERACT.Disable();
        LOOK.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        MoveEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        InteractEvent?.Invoke();
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        LookEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        AttackEvent?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            SprintEvent?.Invoke();
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            SprintCancelledEvent?.Invoke();
        }
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            ReloadEvent?.Invoke();
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            ReloadCancelledEvent?.Invoke();
        }
    }

    private void OnHotkeyPerformed(InputAction.CallbackContext ctx)
    {
        HotkeyEvent?.Invoke(ctx.ReadValue<Vector2>());
    }
}
