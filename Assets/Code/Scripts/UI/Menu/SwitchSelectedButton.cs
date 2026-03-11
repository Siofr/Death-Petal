using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchSelectedButton : MonoBehaviour
{
    private EventSystem _eventSystem;

    private void OnEnable()
    {
        _eventSystem = FindObjectOfType<EventSystem>();
    }
    public void JumpToElement(GameObject targetButton)
    {
        _eventSystem.SetSelectedGameObject(targetButton);
    }
}
