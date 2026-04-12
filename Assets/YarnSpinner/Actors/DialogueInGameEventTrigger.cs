using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogueInGameEventTrigger : MonoBehaviour
{
    public UnityEvent dialogueEvent;

    [YarnCommand("event")]
    public void InvokeEvent()
    {
        dialogueEvent.Invoke();
    }
}
