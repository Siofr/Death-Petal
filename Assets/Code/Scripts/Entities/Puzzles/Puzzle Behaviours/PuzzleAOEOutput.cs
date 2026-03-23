using System.Collections;
using UnityEngine;

public class PuzzleAOEOutput : PuzzleOutputBase
{
    [Header("AOE Output Setup")]
    [SerializeField] private AOEEffect _aoeEffect;
    
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        base.OnPuzzleSolved(context);
        
        _aoeEffect.StartEffect();
    }

    public override void OnPuzzleReset(PuzzleResetEvent context)
    {
        base.OnPuzzleReset(context);
        
        _aoeEffect.EndEffect();
    }
}
