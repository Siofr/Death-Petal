using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PuzzleTriggerInput: PuzzleInputBase
{
    [Space]
    [Header("Puzzle Trigger Fields")]
    [SerializeField] private bool _isOneTimeUse;
    
    private void OnTriggerEnter(Collider other)
    {
        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(other.CompareTag("Player"), output);
        }
    }
}

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

public abstract class PuzzleCameraCondition_SO : ScriptableObject
{
    public Func<bool> exitCondition;
}

public class PuzzleCameraFirstEncounter : PuzzleCameraCondition_SO
{
    [SerializeField] private EnemyBase _targetEnemy;
    
    private void Awake()
    {
        exitCondition = new Func<bool>(ExitCondition);
    }

    private bool ExitCondition()
    {
        return !_targetEnemy.animator.GetBool("Spawning");
    }
}