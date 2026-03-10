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

public struct ExitDialogueEvent : IEvent
{

}

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueRunner dialogueRunner;
    private EventBindings<TriggerDialogueEvent> _triggerDialogueEventListener;

    protected override void Awake()
    {
        base.Awake();
        _triggerDialogueEventListener = new EventBindings<TriggerDialogueEvent>(OnDialogueTrigger);
    }

    private void OnEnable()
    {
        EventBus<TriggerDialogueEvent>.Register(_triggerDialogueEventListener);
    }

    private void OnDisable()
    {
        EventBus<TriggerDialogueEvent>.Unregister(_triggerDialogueEventListener);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueEnd);
    }

    public void OnDialogueTrigger(TriggerDialogueEvent ctx)
    {
        if (ctx.yarnNodeName == null) return;

        dialogueRunner.StartDialogue(ctx.yarnNodeName);
    }

    private void OnDialogueEnd()
    {
        EventBus<ExitDialogueEvent>.Raise(new ExitDialogueEvent());
    }
}
