using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class PuzzleOutputBase : MonoBehaviour, IPuzzleOutput
{
    //Base Fields
    [Header("Base Fields")]
    [SerializeField] private PuzzleOutputData _saveData;
    [SerializeField] private bool _isSolved;
    public Animator animator;
    
    //Events
    private EventBindings<PuzzleSolvedEvent> _puzzleSolvedEventListener;
    private EventBindings<PuzzleResetEvent> _puzzleResetEventListener;
    
    //Properties
    public PuzzleOutputData SaveInfo => _saveData;
    
    public bool IsSolved
    {
        get => _isSolved;
        set => _isSolved = value;
    }

    public virtual void Awake()
    {
        _puzzleSolvedEventListener = new EventBindings<PuzzleSolvedEvent>(OnPuzzleSolved);
        _puzzleResetEventListener = new EventBindings<PuzzleResetEvent>(OnPuzzleReset);
    }

    public void OnEnable()
    {
        EventBus<PuzzleSolvedEvent>.Register(_puzzleSolvedEventListener);
        EventBus<PuzzleResetEvent>.Register(_puzzleResetEventListener);
    }

    public virtual void OnDisable()
    {
        EventBus<PuzzleSolvedEvent>.Unregister(_puzzleSolvedEventListener);
        EventBus<PuzzleResetEvent>.Unregister(_puzzleResetEventListener);
    }
    
    public virtual void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if ((PuzzleOutputBase)context.puzzleOutput != this) return;
        
        animator.SetBool(Animator.StringToHash("IsSolved"), true);
        IsSolved = true;
    }

    public virtual void OnPuzzleReset(PuzzleResetEvent context)
    {
        if ((PuzzleOutputBase)context.puzzleOutput != this) return;
        
        animator.SetBool(Animator.StringToHash("IsSolved"), false);
        IsSolved = false;
    }
}