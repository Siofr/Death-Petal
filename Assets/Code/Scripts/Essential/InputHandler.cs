using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class InputHandler : Singleton<InputHandler>
{
    private InputSystem_Actions _inputActions;

    public static InputAction MOVE;
    public static InputAction INTERACT;
    public static InputAction LOOK;
    public static InputAction AIM;
    public static InputAction ATTACK;
    public static InputAction RELOAD; //depricated
    public static InputAction HOTKEY; //depricated
    public static InputAction SPRINT;
    public static InputAction RESTART; //debug
    public static InputAction PAUSE;

    // Face Buttons
    public static InputAction NORTH;
    public static InputAction WEST;
    public static InputAction EAST;
    public static InputAction SOUTH;

    // Barrel Turning

    public static InputAction BARLEFT;
    public static InputAction BARRIGHT;

    public static event Action PauseEvent;
    
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

    private EventBindings<UnlockInput> _unlockInputListener;
    private EventBindings<LockInput> _lockInputListener;

    [SerializeField]
    private bool _bypassTutorialProgress;

    private Dictionary<string, InputAction> inputDict = new Dictionary<string, InputAction>();

    protected override void Awake()
    {
        base.Awake();

        _inputActions = new InputSystem_Actions();

        _unlockInputListener = new EventBindings<UnlockInput>(OnUnlockInput);
        _lockInputListener = new EventBindings<LockInput>(OnLockInput);
    }

    void OnEnable()
    {
        LoadUserRebinds();

        MOVE = _inputActions.Player.Move;
        MOVE.Enable();
        MOVE.performed += OnMovePerformed;
        inputDict.Add(MOVE.name, MOVE);

        INTERACT = _inputActions.Player.Interact;
        INTERACT.Enable();
        INTERACT.performed += OnInteractPerformed;
        inputDict.Add(INTERACT.name, INTERACT);

        LOOK = _inputActions.Player.Look;
        LOOK.Enable();
        LOOK.performed += OnLookPerformed;
        inputDict.Add(LOOK.name, LOOK);

        ATTACK = _inputActions.Player.Attack;
        ATTACK.Enable();
        ATTACK.performed += OnAttackPerformed;
        inputDict.Add(ATTACK.name, ATTACK);

        // RELOAD = _inputActions.Player.Reload;
        // RELOAD.Enable();
        // RELOAD.performed += OnReload;

        SPRINT = _inputActions.Player.Sprint;
        SPRINT.Enable();
        SPRINT.performed += OnSprint;
        inputDict.Add(SPRINT.name, SPRINT);

        NORTH = _inputActions.Player.North;
        NORTH.Enable();
        NORTH.performed += OnNorthPerformed;
        inputDict.Add(NORTH.name, NORTH);

        WEST = _inputActions.Player.West;
        WEST.Enable();
        WEST.performed += OnWestPerformed;
        inputDict.Add(WEST.name, WEST);

        EAST = _inputActions.Player.East;
        EAST.Enable();
        EAST.performed += OnEastPerformed;
        inputDict.Add(EAST.name, EAST);

        SOUTH = _inputActions.Player.South;
        SOUTH.Enable();
        SOUTH.performed += OnSouthPerformed;
        inputDict.Add(SOUTH.name, SOUTH);

        BARLEFT = _inputActions.Player.BarrelLeft;
        BARLEFT.Enable();
        BARLEFT.performed += OnBarrelLeft;
        inputDict.Add(BARLEFT.name, BARLEFT);

        BARRIGHT = _inputActions.Player.BarrelRight;
        BARRIGHT.Enable();
        BARRIGHT.performed += OnBarrelRight;
        inputDict.Add(BARRIGHT.name, BARRIGHT);

        RESTART = _inputActions.Player.Restart;
        RESTART.Enable();
        RESTART.performed += OnRestartPerformed;
        inputDict.Add(RESTART.name, RESTART);

        PAUSE = _inputActions.Player.Pause;
        PAUSE.Enable();
        PAUSE.performed += OnPausePerformed;
        inputDict.Add(PAUSE.name, PAUSE);
        
        Debug.Log("LockInputInit");
        EventBus<UnlockInput>.Register(_unlockInputListener);
        EventBus<LockInput>.Register(_lockInputListener);
    }

    private void OnDisable()
    {
        MOVE.Disable();
        INTERACT.Disable();
        LOOK.Disable();
        ATTACK.Disable();
        //RELOAD.Disable();
        SPRINT.Disable();

        NORTH.Disable();
        WEST.Disable();
        EAST.Disable();
        SOUTH.Disable();

        BARLEFT.Disable();
        BARLEFT.Disable();
        
        PAUSE.Disable();

        EventBus<UnlockInput>.Unregister(_unlockInputListener);
        EventBus<LockInput>.Unregister(_lockInputListener);
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        PauseEvent?.Invoke();
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

    //depricated
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
    // new here
    private void OnBarrelLeft(InputAction.CallbackContext ctx)
    {
        RotateBarrelEvent.Invoke(-1);
    }
    private void OnBarrelRight(InputAction.CallbackContext ctx)
    {
        RotateBarrelEvent.Invoke(1);
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
    }

    private void OnRestartPerformed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnUnlockInput(UnlockInput ctx)
    {
        inputDict[ctx.inputAction].Enable();
    }

    private void OnLockInput(LockInput ctx)
    {
        if (_bypassTutorialProgress) return;

        inputDict[ctx.InputAction].Disable();
    }

    void LoadUserRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        _inputActions.LoadBindingOverridesFromJson(rebinds);
    }
}
