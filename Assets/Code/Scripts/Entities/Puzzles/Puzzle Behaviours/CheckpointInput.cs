using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

public class CheckpointInput : PuzzleInputBase
{
    [Header("Checkpoint Fields")]
    [SerializeField] private int _id;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);

        if (PuzzleOutputs.Count > 0)
        {
            EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent("SaveSkull"));
        }
    }

    [YarnCommand("UpdateCheckpoint")]
    public void UpdateCheckpoint(int id)
    {
        if (id != _id) return;
        
        CompletionCondition(true, PuzzleOutputs[0]);
    }
}