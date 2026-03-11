using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public struct TutorialTriggerEvent : IEvent
{
    public List<string> tutorialSteps;

    public TutorialTriggerEvent(List<string> tutorialSteps)
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

    private void Start()
    {
        foreach(var tutorialStep in tutorialSteps)
        {
            // tutorialStep.action.
            _tutorialText.Add(tutorialStep.tutorialText);
            tutorialStep.actionRef.action.performed += AdvanceTutorial;
        }
    }

    void AdvanceTutorial(InputAction.CallbackContext ctx)
    {
        EventBus<AdvanceTutorialEvent>.Raise(new AdvanceTutorialEvent(ctx.action.name));
    }

    void TriggerTutorial()
    {
        EventBus<TutorialTriggerEvent>.Raise(new TutorialTriggerEvent(_tutorialText));
    }

    void EndTutorial()
    {
        foreach(var tutorialStep in tutorialSteps)
        {
            tutorialStep.actionRef.action.performed -= AdvanceTutorial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            TriggerTutorial();
        }
    }
}
