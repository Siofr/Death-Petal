using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchSelectedButton : MonoBehaviour
{
    public EventSystem _eventSystem;

    // private void OnEnable()
    // {
    //     _eventSystem = FindAnyObjectByType<EventSystem>();
    // }
    
    public void JumpToElement(GameObject targetButton)
    {
        _eventSystem.SetSelectedGameObject(targetButton);
    }
}
