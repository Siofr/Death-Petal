using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PausingFunc : MonoBehaviour
{
    public bool isPaused;
    public Image UISludge;
    
    public void TogglePause()
    {
        // isPaused = !isPaused;
        // print("b");
        // StartCoroutine(EnterUISludge());
        //
        // if(!isPaused)
        
        EventBus<PauseEvent>.Raise(new PauseEvent(false));
    }

    
    private IEnumerator EnterUISludge()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime * 4f)
        {
            print("a");
            UISludge.material.SetFloat("_LerpIn", i);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(EnterUISludge());
    }

    private void Awake()
    {
        // _pauseEventListener = new EventBindings<PauseEvent>(OnPause);
        // EventBus<PauseEvent>.Register(_pauseEventListener);
    }
    
    private void OnDisable()
    {
        // EventBus<PauseEvent>.Unregister(_pauseEventListener);
        // StopAllCoroutines();
    }
    void Update()
    {
        
    }
}
