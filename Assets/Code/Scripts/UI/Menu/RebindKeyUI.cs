using UnityEngine;
using UnityEngine.InputSystem;

public class RebindKeyUI : MonoBehaviour
{
    public InputActionReference inputActionRef;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    public InputActionAsset inputAsset;

    public void RebindKeyStart()
    {
        inputAsset.Disable();

        _rebindingOperation = inputActionRef.ToInputAction().PerformInteractiveRebinding().OnComplete(operation => RebindKeyFinish());

        _rebindingOperation.Start();
    }

    public void RebindKeyFinish()
    {
        _rebindingOperation.Dispose();

        inputAsset.Enable();
    }
}
