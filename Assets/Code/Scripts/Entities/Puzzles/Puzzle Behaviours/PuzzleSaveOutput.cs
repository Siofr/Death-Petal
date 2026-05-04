using UnityEngine;

public class PuzzleSaveOutput : PuzzleOutputBase
{
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if (IsSolved) return;
        
        EventBus<LevelSaveEvent>.Raise(new LevelSaveEvent());
        base.OnPuzzleSolved(context);
    }

    protected override void Start()
    {
        if (IsSolved) return;
    }
}
