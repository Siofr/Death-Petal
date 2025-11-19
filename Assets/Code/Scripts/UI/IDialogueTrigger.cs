using Yarn.Unity;

public interface IDialogueTrigger
{
    string NodeName { get; set; }
    DialogueRunner Runner { get; set; }

    public void Trigger(string node);
}
