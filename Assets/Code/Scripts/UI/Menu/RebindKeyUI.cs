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
        LoadUserRebinds();
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

        SaveUserRebinds();
    }

    public void SetButtonNameToBinding()
    {
        int bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);

        //buttonTextTMP.text = InputControlPath.ToHumanReadableString(inputActionRef.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //var bindingIndex = inputActionRef.action.GetBindingIndex(InputBinding.MaskByGroups("XInputControllerWindows"));
    
        string rawName = inputActionRef.action.GetBindingDisplayString(bindingIndex);

        if (InputTextToTMPIcon.TryConvertToXboxIcon(out string o, rawName))
        {
            buttonTextTMP.text = o;
        } 
        else
        {
            buttonTextTMP.text = rawName;
        }

    }

    public void resetBinding()
    {
        //inputAsset.Disable();
        //buttonTextTMP.text = waitText;

        int bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);

        inputActionRef.action.RemoveBindingOverride(bindingIndex);

        //inputAsset.Enable();

        SetButtonNameToBinding();

        SaveUserRebinds();
    }


    void SaveUserRebinds()
    {
        var rebinds = inputAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    void LoadUserRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        inputAsset.LoadBindingOverridesFromJson(rebinds);
    }
}
