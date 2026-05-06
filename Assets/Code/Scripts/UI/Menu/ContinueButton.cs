using System;
using UnityEngine;
using UnityEngine.UI;

public struct ToggleSavingEvent : IEvent
{
    public bool canSaveLoad;
    
    public ToggleSavingEvent(bool canSaveLoad)
    {
        this.canSaveLoad = canSaveLoad;
    }
}

public class ContinueButton : MonoBehaviour
{
    private Button _button;
    
    private EventBindings<ToggleSavingEvent> _noSaveLoadEventListener;

    public bool isSaveButton;
    public bool isPauseMenu;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _noSaveLoadEventListener = new EventBindings<ToggleSavingEvent>(OnToggleSaving);

        if(!isSaveButton) _button.interactable = SaveSystem.CheckData();
    }

    //DEPRECATED
    private void OnDestroy()
    {
        EventBus<ToggleSavingEvent>.Unregister(_noSaveLoadEventListener);
    }
    
    //DEPRECATED
    private void OnToggleSaving(ToggleSavingEvent ctx)
    {
        Debug.Log("Entered Button?");
        _button.interactable = ctx.canSaveLoad;
    }
}
