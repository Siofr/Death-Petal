using System.Collections;
using UnityEngine;

public class PuzzleAOEOutput : PuzzleOutputBase
{
    [Header("AOE Output Setup")]
    [SerializeField] private AOEEffect _aoeEffect;
    
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if (context.puzzleOutput != this) return;
        
        base.OnPuzzleSolved(context);
        
        _aoeEffect.StartEffect();
    }

    public override void OnPuzzleReset(PuzzleResetEvent context)
    {
        if (context.puzzleOutput != this) return;
        
        base.OnPuzzleReset(context);
        
        _aoeEffect.EndEffect();
    }
}
