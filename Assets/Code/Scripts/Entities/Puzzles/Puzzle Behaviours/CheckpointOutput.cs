using UnityEngine;

public class CheckpointOutput : PuzzleOutputBase
{
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        base.OnPuzzleSolved(context);
        
        EventBus<SaveEvent>.Raise(new SaveEvent());
    }
}