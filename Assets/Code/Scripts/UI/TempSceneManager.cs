using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct OnMenuLoaded : IEvent
{

}

public class TempSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _creditsButton;
    [SerializeField] private TMP_Text _saveText;
    private Coroutine _saveTextRoutine;
    public bool isCreditsOpen = false;
    private float awakeTime;

    private SwitchSelectedButton _switchButton;
    
    private EventBindings<LevelSaveCompleteEvent> _saveCompleteListener;

    private void OnEnable()
    {
        _saveCompleteListener = new EventBindings<LevelSaveCompleteEvent>(SaveComplete);
        
        EventBus<LevelSaveCompleteEvent>.Register(_saveCompleteListener);
    }

    void OnDisable()
    {
        EventBus<LevelSaveCompleteEvent>.Unregister(_saveCompleteListener);
        ResetSaveText();
    }

    private void Start()
    {
        awakeTime = Time.time;
        _switchButton = GetComponent<SwitchSelectedButton>();
    }

    public void reloadScene()
    {
        //EventBusUtils.ClearAllBuses();
        EventBus<LevelLoadEvent>.Raise(new LevelLoadEvent(false));
        EventBusUtils.ClearAllBuses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void loadDefaultScene(int sceneIndex)
    {
        if (sceneIndex == 1)
        {
            EventBus<LevelLoadEvent>.Raise(new LevelLoadEvent(true));
            LevelManager.isLoadingDefault = true;
        }
        StartCoroutine(LoadSceneWithTransition(sceneIndex));
    }

    public void loadScene(int sceneIndex)
    {
        if (sceneIndex == 1)
        {
            EventBus<LevelLoadEvent>.Raise(new LevelLoadEvent(false));
            LevelManager.isLoadingDefault = false;
        }
        
        if(sceneIndex == 0) EventBus<OnMenuLoaded>.Raise(new OnMenuLoaded());
        
        StartCoroutine(LoadSceneWithTransition(sceneIndex));
    }

    private void SaveComplete()
    {
        _saveText.text = "Saved!";
        _saveText.color = Color.black;
        if(_saveTextRoutine != null)
            StopCoroutine(_saveTextRoutine);
        _saveTextRoutine = StartCoroutine(RevertSavedText());
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator RevertSavedText()
    {
        yield return new WaitForSeconds(1.5f);
        ResetSaveText();
        _saveTextRoutine = null;
    }

    private void ResetSaveText()
    {
        if (_saveText == null) return;
            
        _saveText.color = Color.white;
        _saveText.text = "Save";
    }

    public void save()
    {
        EventBus<LevelSaveEvent>.Raise(new LevelSaveEvent());
    }

    private IEnumerator LoadSceneWithTransition(int sceneIndex)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true, true));
        yield return new WaitForSeconds(0.3f);

        EventBusUtils.ClearAllBuses();
        
        yield return SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        SceneManager.UnloadSceneAsync(activeScene);
        //EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(false, true));
    }

    public void LoadMainMenu()
    {
        if(Time.time - awakeTime < 3.6f) return;
        StartCoroutine(ReturnToMainMenuWithTransition());
    }
    
    private IEnumerator ReturnToMainMenuWithTransition()
    {
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true, true));
        yield return new WaitForSeconds(0.5f);
        
        // temporarily goes to gradXscene
        yield return SceneManager.LoadSceneAsync(0);
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
