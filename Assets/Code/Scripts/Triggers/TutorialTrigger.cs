using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public struct TutorialTriggerEvent : IEvent
{
    public Dictionary<string, string> tutorialSteps;

    public TutorialTriggerEvent(Dictionary<string, string> tutorialSteps)
    {
        this.tutorialSteps = tutorialSteps;
    }
}

public struct AdvanceTutorialEvent : IEvent
{
    public string actionName;

    public AdvanceTutorialEvent(string actionName)
    {
        this.actionName = actionName;
    }
}

public struct EndTutorialEvent : IEvent
{

}

public struct UnlockInput : IEvent
{
    public string inputAction;

    public UnlockInput(string inputAction)
    {
        this.inputAction = inputAction;
    }
}

public struct LockInput : IEvent
{
    public string InputAction;

    public LockInput(string inputAction)
    {
        this.InputAction = inputAction;
    }
}

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    private bool _unlocksInputs;

    [System.Serializable]
    public struct TutorialInfo
    {
        public string tutorialText;
        public InputActionReference actionRef;
    }

    public List<TutorialInfo> tutorialInfo = new List<TutorialInfo>();

    private int _tutorialIndex;
    private int _stepIndex;
    private bool _playOnce = false;
    private List<string> _tutorialText = new List<string>();
    private List<InputActionReference> _actionList = new List<InputActionReference>();

    private Dictionary<string, string> _stepsDict = new Dictionary<string, string>();

    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        if (_playOnce) return;

        if (!_unlocksInputs) return;

        LockActions();
    }

    void AdvanceTutorial(InputAction.CallbackContext ctx)
    {
        ctx.action.performed -= AdvanceTutorial;
        _stepIndex++;
        EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent(ctx.action.name));

        if (_stepIndex >= tutorialInfo.Count)
        {
            EndTutorial();
            return;
        }

        _actionList[_stepIndex].action.performed += AdvanceTutorial;
        if (_unlocksInputs) EventBus<UnlockInput>.Raise(new UnlockInput(_actionList[_stepIndex].action.name));
    }

    public void TriggerTutorial(int tutorialIndex)
    {
        if (tutorialInfo.Count == 0) return;

        foreach (var tutorialStep in tutorialInfo)
        {
            _actionList.Add(tutorialStep.actionRef);
            string inputName = tutorialStep.actionRef.name;
            string output = inputName.Substring(inputName.IndexOf('/') + 1);
            string binds = SetButtonNameToBinding(tutorialStep.actionRef);
            _stepsDict.Add(binds + " - " + tutorialStep.tutorialText, output);
        }

        _actionList[0].action.performed += AdvanceTutorial;

        EventBus<TutorialTriggerEvent>.Raise(new TutorialTriggerEvent(_stepsDict));

        if (_unlocksInputs) EventBus<UnlockInput>.Raise(new UnlockInput(_actionList[0].action.name));
    }

    void EndStep()
    {
        foreach(var tutorialStep in tutorialInfo)
        {
            tutorialStep.actionRef.action.performed -= AdvanceTutorial;
        }

        _stepIndex = 0;
        _tutorialIndex++;
        _stepsDict.Clear();

        // EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent());

        if (_tutorialIndex < tutorialInfo.Count)
        {
            TriggerTutorial(_tutorialIndex);
        }
        else
        {
            EndTutorial();
        }
    }

    void EndTutorial()
    {
        _stepIndex = 0;
        _stepsDict.Clear();
        EventBus<EndTutorialEvent>.Raise(new EndTutorialEvent());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playOnce) return;

        if (other.transform.tag == "Player")
        {
            _playOnce = true;
            TriggerTutorial(_tutorialIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = _tutorialIndex; i < tutorialInfo.Count; i++)
        {
            EventBus<UnlockInput>.Raise(new UnlockInput(_actionList[i].action.name));
        }

        EndTutorial();
    }

    public string SetButtonNameToBinding(InputActionReference inputActionRef)
    {
        int bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);

        string rawName = inputActionRef.action.GetBindingDisplayString(bindingIndex);

        if (InputTextToTMPIcon.TryConvertToXboxIcon(out string o, rawName))
        {
            return o;
        }
        else
        {
            return rawName;
        }

    }

    public void LockActions()
    {
        if (!LevelManager.isLoadingDefault) return;
        
        foreach(TutorialInfo info in tutorialInfo)
        {
            EventBus<LockInput>.Raise(new LockInput(info.actionRef.action.name));
        }
    }
}
