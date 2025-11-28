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
            var yarnVariableManager =  GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
            float saveSkullID;
            
            yarnVariableManager.TryGetValue("$saveSkullID", out saveSkullID);
            saveSkullID = _id;
            
            yarnVariableManager.SetValue("$saveSkullID", saveSkullID);
            
            EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent("SaveSkull"));
        }
    }

    [YarnCommand("update_checkpoint(saveSkullID)")]
    public void UpdateCheckpoint(float id)
    {
        if (_id != id) return;
        
        CompletionCondition(true, PuzzleOutputs[0]);
    }
}