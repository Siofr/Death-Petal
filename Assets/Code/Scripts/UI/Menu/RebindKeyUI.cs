using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindKeyUI : MonoBehaviour
{
    public InputActionReference inputActionRef;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    public InputActionAsset inputAsset;
    public TextMeshProUGUI buttonTextTMP; 
    public string waitText;

    void OnEnable()
    {
        SetButtonNameToBinding();
    }

    public void RebindKeyStart()
    {
        inputAsset.Disable();
        buttonTextTMP.text = waitText;

        _rebindingOperation = inputActionRef.action.PerformInteractiveRebinding().OnComplete(operation => RebindKeyFinish());

        _rebindingOperation.Start();
    }

    void RebindKeyFinish()
    {
        _rebindingOperation.Dispose();

        inputAsset.Enable();

        SetButtonNameToBinding();
    }

    void SetButtonNameToBinding()
    {
        int bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);

        //buttonTextTMP.text = InputControlPath.ToHumanReadableString(inputActionRef.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //var bindingIndex = inputActionRef.action.GetBindingIndex(InputBinding.MaskByGroups("XInputControllerWindows"));
    
        buttonTextTMP.text = inputActionRef.action.GetBindingDisplayString(bindingIndex);
    }
}
