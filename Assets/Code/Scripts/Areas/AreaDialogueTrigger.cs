using UnityEngine;
using Yarn.Unity;

public class AreaDialogueTrigger : MonoBehaviour, IDialogueTrigger
{
    public string nodeName;

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
        Runner = Object.FindFirstObjectByType<DialogueRunner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Trigger(NodeName);
        }
    }

    public void Trigger(string node)
    {
        Runner.StartDialogue(node);
    }
}
