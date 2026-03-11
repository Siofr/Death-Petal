using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchSelectedButton : MonoBehaviour
{
    public EventSystem eventSystem;

    public void JumpToElement(Button targetButton)
    {
        eventSystem.SetSelectedGameObject(targetButton.gameObject);
    }
}
