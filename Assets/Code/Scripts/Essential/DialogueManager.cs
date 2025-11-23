using UnityEngine;
using Yarn.Unity;

public struct TriggerDialogueEvent : IEvent
{
    public string yarnNodeName;

    public TriggerDialogueEvent(string newNode)
    {
        this.yarnNodeName = newNode;
    }
}

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueRunner dialogueRunner;
    private EventBindings<TriggerDialogueEvent> _triggerDialogueEventListener;

    private void Awake()
    {
        base.Awake();
        _triggerDialogueEventListener = new EventBindings<TriggerDialogueEvent>(OnDialogueTrigger);
    }

    private void OnEnable()
    {
        EventBus<TriggerDialogueEvent>.Register(_triggerDialogueEventListener);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void OnDialogueTrigger(TriggerDialogueEvent ctx)
    {
        dialogueRunner.StartDialogue(ctx.yarnNodeName);
    }
}
