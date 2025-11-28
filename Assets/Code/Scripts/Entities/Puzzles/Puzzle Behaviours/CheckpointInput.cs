using System;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

public struct CheckpointActivatedEvent : IEvent
{
    public CheckpointInput checkpointInput;
    public CheckpointActivatedEvent(CheckpointInput checkpointInput) => this.checkpointInput = checkpointInput;
}

public class CheckpointInput : PuzzleInputBase
{
    [Header("Checkpoint Fields")]
    [SerializeField] private Animator _skullAnimator;
    private static CheckpointInput _activeCheckpoint;
    
    //Events
    private EventBindings<CheckpointActivatedEvent> _checkpointActivatedListener;

    protected void OnEnable()
    {
        _checkpointActivatedListener = new EventBindings<CheckpointActivatedEvent>(CheckCondition);
        EventBus<CheckpointActivatedEvent>.Register(_checkpointActivatedListener);
    }

    protected void OnDisable()
    {
        EventBus<CheckpointActivatedEvent>.Unregister(_checkpointActivatedListener);
    }

    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);

        if (PuzzleOutputs.Count > 0)
        {
            tag = "Checkpoint";
            
            _skullAnimator.SetBool(Animator.StringToHash("IsActivated"), true);
            EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent("SaveSkull"));
        }
    }

    public void CheckCondition(CheckpointActivatedEvent context)
    {
        tag = "Untagged";
        
        if(this != context.checkpointInput) CompletionCondition(false, PuzzleOutputs[0]);
        else  CompletionCondition(true, PuzzleOutputs[0]);
    }

    [YarnCommand("update_checkpoint")]
    static public void UpdateCheckpoint(string checkpointName)
    {
        var input = GameObject.FindWithTag(checkpointName).GetComponent<CheckpointInput>();
        
        EventBus<CheckpointActivatedEvent>.Raise(new CheckpointActivatedEvent(input));
        EventBus<LevelSaveEvent>.Raise(new LevelSaveEvent());
    }
}