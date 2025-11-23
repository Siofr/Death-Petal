using UnityEngine;

public struct CheckpointShotEvent : IEvent
{
    public IPuzzleOutput checkpointOutput;

    public CheckpointShotEvent(IPuzzleOutput checkpointOutputRef) =>  checkpointOutput = checkpointOutputRef;
}

public class CheckpointInput : PuzzleInputBase
{
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);

        if (PuzzleOutputs.Count > 0)
        {
            EventBus<CheckpointShotEvent>.Raise(new CheckpointShotEvent(PuzzleOutputs[0]));           
        }
    }
}

public class CheckpointOutput : PuzzleOutputBase { }