using UnityEngine;

public class PuzzleCameraOutput : PuzzleOutputBase
{
    [Space]
    [Header("Camera Condition Fields")]
    [SerializeField] private PuzzleCameraCondition_SO _puzzleCameraCondition;

    [Space]
    [Header("Animator Conditions")]
    [SerializeField] private Animator _animator;
    
    

    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        base.OnPuzzleSolved(context);

        if (context.puzzleOutput != this) return;

        StartPanningCamera(_puzzleCameraCondition.exitCondition);
    }

    public override void OnPuzzleBoundsEntered()
    {
        base.OnPuzzleBoundsEntered();
        if(_animator) _animator.SetTrigger("Play");
        _puzzleCameraCondition.enteredTime = Time.time;
    }
}