using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelDownInput : MonoBehaviour
{
    private EventSystem eventSystem;
    private EventTrigger eventTrigger;

    private void Awake()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
    }

    public Selectable exitPoint;

    public void BackOutOfMenu()
    {
        eventSystem.SetSelectedGameObject(exitPoint.gameObject);        
    }

    private bool IsSelectedChild()
    {
        Transform[] childComponents = GetComponentsInChildren<Transform>();
        foreach (var selectable in childComponents)
        {
            if (selectable.GetComponent<Selectable>() != null && eventSystem.currentSelectedGameObject == selectable.gameObject)
            {
                return true;
            }
        }
        return false;
    }
    
    
}
