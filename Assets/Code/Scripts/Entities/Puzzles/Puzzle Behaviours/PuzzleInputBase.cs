using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleInputBase : MonoBehaviour, IPuzzleInput
{
    [Header("Base Puzzle Input Fields")] 
    [SerializeField] private Transform _outputTransform;
    
    //Non-Serializable Fields
    private IPuzzleOutput _puzzleOutput;
    private List<Weakness> _weaknesses;
    
    //Properties
    public IPuzzleOutput PuzzleOutput => _puzzleOutput;
    public List<Weakness> Weaknesses => _weaknesses;

    private void Awake()
    {
        _outputTransform.TryGetComponent(out _puzzleOutput);
    }

    public abstract void OnShot(Weakness damageType);


    public bool CompletionCondition(Func<bool> condition, bool reset)
    {
        var result = condition.Invoke();

        if (!result && reset)
        {
            EventBus<PuzzleResetEvent>.Raise(new  PuzzleResetEvent());
            return false;
        }
        
        EventBus<PuzzleSolvedEvent>.Raise(new PuzzleSolvedEvent());
        
        return result;
    }
}

