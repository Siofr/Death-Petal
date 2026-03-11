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

    public List<TutorialInfo> tutorialSteps = new List<TutorialInfo>();

    private List<string> _tutorialText = new List<string>();
    private Dictionary<string, string> _stepsDict = new Dictionary<string, string>();

    private void Start()
    {
        foreach(var tutorialStep in tutorialSteps)
        {
            _tutorialText.Add(tutorialStep.tutorialText);
            tutorialStep.actionRef.action.performed += AdvanceTutorial;
            string inputName = tutorialStep.actionRef.name;
            string output = inputName.Substring(inputName.IndexOf('/') + 1);
            _stepsDict.Add(tutorialStep.tutorialText, output);
        }
    }

    void AdvanceTutorial(InputAction.CallbackContext ctx)
    {
        EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent(ctx.action.name));
    }

    void TriggerTutorial()
    {
        EventBus<TutorialTriggerEvent>.Raise(new TutorialTriggerEvent(_stepsDict));
    }

    void EndTutorial()
    {
        foreach(var tutorialStep in tutorialSteps)
        {
            tutorialStep.actionRef.action.performed -= AdvanceTutorial;
        }

        EventBus<EndTutorialEvent>.Raise(new EndTutorialEvent());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            TriggerTutorial();
        }
    }
}
