using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleInputBase : EntityBase, IPuzzleInput
{
    [Header("Base Puzzle Input Fields")] 
    [SerializeField] private List<GameObject> _outputObjects = new  List<GameObject>();
    
    //Non-Serializable Fields
    private List<IPuzzleOutput> _puzzleOutputs = new List<IPuzzleOutput>();
    //Properties
    public List<IPuzzleOutput> PuzzleOutputs => _puzzleOutputs;

    protected override void Awake()
    {
        base.Awake();
        
        foreach (var outputs in _outputObjects)
        {
            if(outputs.TryGetComponent<PuzzleOutputBase>(out var puzzleOutput)) _puzzleOutputs.Add(puzzleOutput);
        }
        
        Debug.Log(PuzzleOutputs.Count);
    }

    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;
    }

    public bool CompletionCondition(bool condition, IPuzzleOutput targetOutput)
    {
        //Debug.Log("Evaluating Puzzle Conditions");

        if (!_puzzleOutputs.Contains(targetOutput)) return false;
        
        if (!condition)
        {
            if(targetOutput.IsSolved) EventBus<PuzzleResetEvent>.Raise(new  PuzzleResetEvent(targetOutput));
            return false;
        }
        
        if(!targetOutput.IsSolved) EventBus<PuzzleSolvedEvent>.Raise(new PuzzleSolvedEvent(targetOutput));
        
        return true;
    }
}

