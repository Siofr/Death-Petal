using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplaceInactiveNav : MonoBehaviour
{
    EventSystem eventSystem;
    Selectable selfSelectable;
    void Start()
    {
        eventSystem = EventSystem.current;
        selfSelectable = GetComponent<Selectable>();
        
        CheckForInactiveButton();
    }

    private void CheckForInactiveButton()
    {
        Navigation nav = selfSelectable.navigation;
        
        //Selectable up = nav.selectOnUp.GetComponent<Selectable>();
        //Selectable down = nav.selectOnDown.GetComponent<Selectable>();
        //Selectable right = nav.selectOnRight.GetComponent<Selectable>();
        //Selectable left = nav.selectOnLeft.GetComponent<Selectable>();
        
        if (nav.selectOnUp.TryGetComponent(out Selectable up))
        {
            if (!up.interactable) nav.selectOnUp = up.navigation.selectOnUp;
        }
        if (nav.selectOnDown.TryGetComponent(out Selectable down))
        {
            if (!down.interactable) nav.selectOnDown = down.navigation.selectOnDown;
        }
        
        selfSelectable.navigation = nav;
    }
}
