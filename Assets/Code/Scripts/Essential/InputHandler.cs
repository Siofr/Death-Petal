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

    // Face Buttons
    public static InputAction NORTH;
    public static InputAction WEST;
    public static InputAction EAST;
    public static InputAction SOUTH;

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

    public static event Action<int> HotkeyEvent;
    public static event Action RemoveBulletEvent;

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

        RELOAD = _inputActions.Player.Reload;
        RELOAD.Enable();
        RELOAD.performed += OnReload;

        SPRINT = _inputActions.Player.Sprint;
        SPRINT.Enable();
        SPRINT.performed += OnSprint;

        NORTH = _inputActions.Player.North;
        NORTH.Enable();
        NORTH.performed += OnNorthPerformed;

        WEST = _inputActions.Player.West;
        WEST.Enable();
        WEST.performed += OnWestPerformed;

        EAST = _inputActions.Player.East;
        EAST.Enable();
        EAST.performed += OnEastPerformed;

        SOUTH = _inputActions.Player.South;
        SOUTH.Enable();
        SOUTH.performed += OnSouthPerformed;

        RESTART = _inputActions.Player.Restart;
        RESTART.Enable();
        RESTART.performed += OnRestartPerformed;
    }

    private void OnDisable()
    {
        MOVE.Disable();
        INTERACT.Disable();
        LOOK.Disable();
        ATTACK.Disable();
        RELOAD.Disable();
        SPRINT.Disable();
        NORTH.Disable();
        WEST.Disable();
        EAST.Disable();
        SOUTH.Disable();
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

    private void OnNorthPerformed(InputAction.CallbackContext ctx)
    {
        HotkeyEvent.Invoke(2);
    }

    private void OnEastPerformed(InputAction.CallbackContext ctx)
    {
        HotkeyEvent.Invoke(1);
    }

    private void OnWestPerformed(InputAction.CallbackContext ctx)
    {
        HotkeyEvent.Invoke(0);
    }

    private void OnSouthPerformed(InputAction.CallbackContext ctx)
    {
        RemoveBulletEvent?.Invoke();
        // HotkeyEvent?.Invoke();
    }

    private void OnRestartPerformed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
