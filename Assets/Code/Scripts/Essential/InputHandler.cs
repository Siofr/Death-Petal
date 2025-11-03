using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    public static InputAction MOVE;
    public static InputAction INTERACT;
    public static InputAction LOOK;

    public static event Action<Vector2> OnMoveEvent;
    public static event Action OnInteractEvent;
    public static event Action<Vector2> OnLookEvent;

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
    }

    private void OnDisable()
    {
        MOVE.Disable();
        INTERACT.Disable();
        LOOK.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        OnInteractEvent?.Invoke();
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        OnLookEvent?.Invoke(ctx.ReadValue<Vector2>());
    }
}
