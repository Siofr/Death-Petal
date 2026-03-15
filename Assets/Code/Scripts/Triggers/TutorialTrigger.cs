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

    [System.Serializable]
    public struct TutorialStep
    {
        public List<TutorialInfo> tutorialInfo;
    }

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private int _tutorialIndex;
    private int _stepIndex;
    private List<string> _tutorialText = new List<string>();
    private Dictionary<string, string> _stepsDict = new Dictionary<string, string>();

    void AdvanceTutorial(InputAction.CallbackContext ctx)
    {
        ctx.action.performed -= AdvanceTutorial;
        _stepIndex++;
        EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent(ctx.action.name));

        if (_stepIndex >= tutorialSteps[_tutorialIndex].tutorialInfo.Count)
        {
            EndStep();
        }
    }

    public void TriggerTutorial(int tutorialIndex)
    {
        foreach (var tutorialStep in tutorialSteps[tutorialIndex].tutorialInfo)
        {
            tutorialStep.actionRef.action.performed += AdvanceTutorial;
            string inputName = tutorialStep.actionRef.name;
            string output = inputName.Substring(inputName.IndexOf('/') + 1);
            string binds = tutorialStep.actionRef.action.GetBindingDisplayString();
            _stepsDict.Add(binds + " - " + tutorialStep.tutorialText, output);
        }

        EventBus<TutorialTriggerEvent>.Raise(new TutorialTriggerEvent(_stepsDict));
    }

    void EndStep()
    {
        foreach(var tutorialStep in tutorialSteps[_tutorialIndex].tutorialInfo)
        {
            tutorialStep.actionRef.action.performed -= AdvanceTutorial;
        }

        _stepIndex = 0;
        _tutorialIndex++;
        _stepsDict.Clear();

        // EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent());

        if (_tutorialIndex < tutorialSteps.Count)
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            EndTutorial();
        }
    }
}
