using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleInputBase : MonoBehaviour, IPuzzleInput
{
    [Header("Base Puzzle Input Fields")] 
    [SerializeField] private List<GameObject> _outputObjects = new  List<GameObject>();
    
    //Non-Serializable Fields
    private List<IPuzzleOutput> _puzzleOutputs = new List<IPuzzleOutput>();
    private List<Weakness> _weaknesses;
    
    //Properties
    public List<IPuzzleOutput> PuzzleOutputs => _puzzleOutputs;
    public List<Weakness> Weaknesses => _weaknesses;

    private void Awake()
    {
        foreach (var outputs in _outputObjects)
        {
            if(outputs.TryGetComponent<PuzzleOutputBase>(out var puzzleOutput)) _puzzleOutputs.Add(puzzleOutput);
        }
    }

    public abstract void OnShot(Weakness weakness, WeakTypes damageType);


    public bool CompletionCondition(bool condition, IPuzzleOutput targetOutput)
    {
        Debug.Log("Evaluating Puzzle Conditions");
        
        if (!condition)
        {
            if(targetOutput.IsSolved) EventBus<PuzzleResetEvent>.Raise(new  PuzzleResetEvent(targetOutput));
            return false;
        }
        
        if(!targetOutput.IsSolved) EventBus<PuzzleSolvedEvent>.Raise(new PuzzleSolvedEvent(targetOutput));
        
        return true;
    }
}

