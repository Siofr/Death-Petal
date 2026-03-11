using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class InputHandler : Singleton<InputHandler>
{
    private InputSystem_Actions _inputActions;

    public static InputAction MOVE;
    public static InputAction INTERACT;
    public static InputAction LOOK;
    public static InputAction AIM;
    public static InputAction ATTACK;
    public static InputAction RELOAD;
    public static InputAction HOTKEY;
    public static InputAction SPRINT;
    public static InputAction RESTART;

    public static event Action<Vector2> MoveEvent;
    public static event Action InteractEvent;
    public static event Action<Vector2> LookEvent;
    public static event Action AttackEvent;

    public static event Action<int> RotateBarrelEvent;
    public static event Action ReloadEvent;
    public static event Action LongReloadEvent;
    public static event Action LongReloadCancelledEvent;
    public static event Action QuickReloadEvent;

    public static event Action AimEvent;
    public static event Action AimCancelledEvent;

    public static event Action<Vector2> HotkeyEvent;

    public static event Action SprintEvent;
    public static event Action SprintCancelledEvent;

    protected override void Awake()
    {
        base.Awake();

        _inputActions = new InputSystem_Actions();
    }

    void OnEnable()
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

/*        AIM = _inputActions.Player.Aim;
        AIM.Enable();
        AIM.performed += OnAim;
        AIM.canceled += OnAim;*/

        RELOAD = _inputActions.Player.Reload;
        RELOAD.Enable();
        RELOAD.performed += OnReload;

        SPRINT = _inputActions.Player.Sprint;
        SPRINT.Enable();
        SPRINT.performed += OnSprint;

        HOTKEY = _inputActions.Player.Hotkey;
        HOTKEY.Enable();
        HOTKEY.performed += OnHotkeyPerformed;

        RESTART = _inputActions.Player.Restart;
        RESTART.Enable();
        RESTART.performed += OnRestartPerformed;
    }

    private void OnDisable()
    {
        MOVE.Disable();
        INTERACT.Disable();
        LOOK.Disable();
        AIM.Disable();
        ATTACK.Disable();
        RELOAD.Disable();
        SPRINT.Disable();
        HOTKEY.Disable();
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
        SprintEvent?.Invoke();
    }

    private void OnAim(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            AimEvent?.Invoke();
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            AimCancelledEvent?.Invoke();
        }
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.ReadValue<float>());
        if (ctx.ReadValue<float>() >= 1)
        {
            RotateBarrelEvent.Invoke(1);
            return;
        }

        RotateBarrelEvent.Invoke(-1);
    }

    private void OnHotkeyPerformed(InputAction.CallbackContext ctx)
    {
        HotkeyEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnRestartPerformed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
