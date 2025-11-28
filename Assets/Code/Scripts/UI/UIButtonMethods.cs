using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class UIButtonMethods : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Audio Paths")]
    public EventReference onButtonHoverEventPath;

    public void OnDeselect(BaseEventData eventData)
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        RuntimeManager.PlayOneShot(onButtonHoverEventPath);
    }
}
