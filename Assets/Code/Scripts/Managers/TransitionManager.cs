using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public struct SetTransitionEvent : IEvent
{
    public bool showTransition;
    public bool doAnimation;

    public SetTransitionEvent(bool showTransition, bool doAnimation)
    {
        this.showTransition = showTransition;
        this.doAnimation = doAnimation;
    }
}

public class TransitionManager : MonoBehaviour
{
    private EventBindings<SetTransitionEvent> _SetTransitionEventListener;

    [SerializeField] private Image _transitionDisplayPanel;

    private void Awake()
    {
        _SetTransitionEventListener = new EventBindings<SetTransitionEvent>(OnSetTransitionEvent);
    }

    private void OnEnable()
    {
        EventBus<SetTransitionEvent>.Register(_SetTransitionEventListener);
    }

    private void OnDisable()
    {
        EventBus<SetTransitionEvent>.Unregister(_SetTransitionEventListener);
        _transitionDisplayPanel.material.SetFloat("_MaxEdge", 0);
        _transitionDisplayPanel.material.SetFloat("_MinEdge", 0);
    }

    public void OnSetTransitionEvent(SetTransitionEvent ctx)
    {
        if(ctx.doAnimation)
            StartCoroutine(AnimateTransition(ctx.showTransition));
        else
        {
            if (ctx.showTransition)
            {
                _transitionDisplayPanel.material.SetFloat("_MaxEdge", 1);
                _transitionDisplayPanel.material.SetFloat("_MinEdge", 0);
            }
            else
            {
                _transitionDisplayPanel.material.SetFloat("_MaxEdge", 1);
                _transitionDisplayPanel.material.SetFloat("_MinEdge", 1);
            }
        }
    }

    /*public void DEBUG_ButtonActivate()
    {
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true));
    }
    
    public void DEBUG_ButtonDisable()
    {
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(false));
    }*/

    private IEnumerator AnimateTransition(bool setOn)
    {
        var valueString = setOn ? "_MaxEdge" : "_MinEdge";
        if(setOn) _transitionDisplayPanel.material.SetFloat("_MinEdge", 0);
        else _transitionDisplayPanel.material.SetFloat("_MaxEdge", 1);

        for (float i = 0; i <= 1f; i += Time.deltaTime * 3f)
        {
            _transitionDisplayPanel.material.SetFloat(valueString, i);
            yield return new WaitForFixedUpdate();
        }
    }
}
