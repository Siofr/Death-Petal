using UnityEngine;

public abstract class PuzzleOutputBase : MonoBehaviour, IPuzzleOutput
{
    //Non-Serializable Fields
    private bool _isSolved;
    
    //Events
    private EventBindings<PuzzleSolvedEvent> _puzzleSolvedEventListener;
    private EventBindings<PuzzleResetEvent> _puzzleResetEventListener;
    
    //Properties
    public bool IsSolved
    {
        get => _isSolved;
        set => _isSolved = value;
    }

    public virtual void Awake()
    {
        _puzzleSolvedEventListener = new EventBindings<PuzzleSolvedEvent>(OnPuzzleSolved);
        _puzzleResetEventListener = new EventBindings<PuzzleResetEvent>(OnPuzzleReset);
        
        EventBus<PuzzleSolvedEvent>.Register(_puzzleSolvedEventListener);
        EventBus<PuzzleResetEvent>.Register(_puzzleResetEventListener);
    }

    public virtual void OnDisable()
    {
        EventBus<PuzzleSolvedEvent>.Unregister(_puzzleSolvedEventListener);
        EventBus<PuzzleResetEvent>.Unregister(_puzzleResetEventListener);
    }
    
    public abstract void OnPuzzleSolved(PuzzleSolvedEvent context);
    public abstract void OnPuzzleReset(PuzzleResetEvent context);
}