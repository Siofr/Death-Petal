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

public class TutorialTrigger : MonoBehaviour
{

    [System.Serializable]
    public struct TutorialInfo
    {
        public string tutorialText;
        public InputActionReference actionRef;
    }

    public List<TutorialInfo> tutorialInfo = new List<TutorialInfo>();

    private int _tutorialIndex;
    private int _stepIndex;
    private List<string> _tutorialText = new List<string>();
    private Dictionary<string, string> _stepsDict = new Dictionary<string, string>();
    private BoxCollider _collider;
    private List<InputActionReference> _actionList = new List<InputActionReference>();

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
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
    }

    public void TriggerTutorial(int tutorialIndex)
    {
        Debug.Log("Trigger the tutorial!");

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
        _tutorialIndex = 0;
        _stepIndex = 0;
        _stepsDict.Clear();
        EventBus<EndTutorialEvent>.Raise(new EndTutorialEvent());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            TriggerTutorial(_tutorialIndex);
            _collider.enabled = false;
        }
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
}
