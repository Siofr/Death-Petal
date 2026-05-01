using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _creditsButton;
    public bool isCreditsOpen = false;

    private SwitchSelectedButton _switchButton;

    private void Start()
    {
        _switchButton = GetComponent<SwitchSelectedButton>();
    }

    public void reloadScene()
    {
        EventBusUtils.ClearAllBuses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        EventBus<LevelLoadEvent>.Raise(new LevelLoadEvent(false));
    }

    public void loadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadCredits()
    {
        StartCoroutine(OpenCreditsWithTransition());
    }

    public void CloseCredits()
    {
        StartCoroutine(CloseCreditsWithTransition());
    }

    private IEnumerator OpenCreditsWithTransition()
    {
        isCreditsOpen = true;
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true, true));
        yield return new WaitForSeconds(0.5f);
        _credits.SetActive(true);
        
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(false, true));
    }
    
    private IEnumerator CloseCreditsWithTransition()
    {
        isCreditsOpen = false;
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true, true));
        yield return new WaitForSeconds(0.5f);
        _credits.SetActive(false);
        _switchButton.JumpToElement(_creditsButton);
        
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(false, true));
    }
}
