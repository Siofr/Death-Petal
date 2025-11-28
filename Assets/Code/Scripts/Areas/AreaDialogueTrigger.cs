using UnityEngine;
using Yarn.Unity;

public class AreaDialogueTrigger : MonoBehaviour
{
    public string nodeName;
    [SerializeField] private bool _allowMulitpleTriggers;
    private int _triggerCount;

    public string NodeName
    {
        get { return nodeName; }
        set { nodeName = value; }
    }

    DialogueRunner runner;
    public DialogueRunner Runner
    {
        get { return runner; }
        set { runner = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _triggerCount = 0;
        Runner = Object.FindFirstObjectByType<DialogueRunner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && ((_triggerCount < 1) || (_allowMulitpleTriggers)))
        {
            _triggerCount++;
            Trigger(NodeName);
        }
    }

    public void Trigger(string node)
    {
        EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent(NodeName));
    }
}
