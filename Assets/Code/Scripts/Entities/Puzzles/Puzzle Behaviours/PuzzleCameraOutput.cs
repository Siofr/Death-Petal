using UnityEngine;

public class PuzzleCameraOutput : PuzzleOutputBase
{
    [Space]
    [Header("Camera Condition Fields")]
    [SerializeField] private PuzzleCameraCondition_SO _puzzleCameraCondition;

    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        base.OnPuzzleSolved(context);

        if (context.puzzleOutput != this) return;

        StartPanningCamera(_puzzleCameraCondition.exitCondition);
    }
}