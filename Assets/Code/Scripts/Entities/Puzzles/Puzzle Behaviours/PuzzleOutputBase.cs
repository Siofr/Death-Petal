using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public abstract class PuzzleOutputBase : MonoBehaviour, IPuzzleOutput, ISaveable<PuzzleOutputSaveData>
{
    //Base Fields
    [Header("Base Fields")]
    [SerializeField] private bool _isSolved;
    public Animator animator;
    
    //Events
    private EventBindings<PuzzleSolvedEvent> _puzzleSolvedEventListener;
    private EventBindings<PuzzleResetEvent> _puzzleResetEventListener;

    // SFX
    [Header("Audio Paths")]
    public EventReference onCompletionEventPath;
    
    public bool IsSolved
    {
        get => _isSolved;
        set => _isSolved = value;
    }

    protected virtual void Awake()
    {
        _puzzleSolvedEventListener = new EventBindings<PuzzleSolvedEvent>(OnPuzzleSolved);
        _puzzleResetEventListener = new EventBindings<PuzzleResetEvent>(OnPuzzleReset);
    }

    private void Start()
    {
        if (_isSolved)
        {
            EventBus<PuzzleSolvedEvent>.Raise(new PuzzleSolvedEvent(this));
        }
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

        RuntimeManager.PlayOneShot(onCompletionEventPath, transform.position);
        animator.SetBool(Animator.StringToHash("IsSolved"), true);
        IsSolved = true;
    }

    public virtual void OnPuzzleReset(PuzzleResetEvent context)
    {
        if ((PuzzleOutputBase)context.puzzleOutput != this) return;
        
        animator.SetBool(Animator.StringToHash("IsSolved"), false);
        IsSolved = false;
    }
    
    //Saving Stuff
    
    private PuzzleOutputSaveData _saveData;
    private int _saveID; 
        
    public PuzzleOutputSaveData SaveInfo => _saveData;
    public int SaveID => _saveID;

    public void CreateSaveInstance()
    {
        _saveID = ISaveableHelper.GenerateISaveableID();
        
        _saveData = new PuzzleOutputSaveData(_saveID, _isSolved);
    }

    public void DeleteSaveInstance()
    {
        if (SaveID == 0) return;
        ISaveableHelper.RemoveExistingID(ref _saveID);

        _saveData = new PuzzleOutputSaveData();
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        _saveData.Save(IsSolved);
        
        for (var i = 0; i < refData.puzzleOutputSaveData.Count; i++)
        {
            if (refData.puzzleOutputSaveData[i].id != SaveID) continue;
            
            refData.puzzleOutputSaveData[i] = _saveData;
            return;
        }
        
        refData.puzzleOutputSaveData.Add(_saveData);
    }

    public void HandleLoadData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID))
        {
            Debug.Log("No Puzzle ID in Saveables");
            return;
        }
        
        foreach (var data in refData.puzzleOutputSaveData)
        {
            if (data.id != SaveID) continue;

            _saveData = data;
            
            var output = GetComponent<IPuzzleOutput>();
            
            _saveData.Load(ref output);
            return;
        }
    }
}