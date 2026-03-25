using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;

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
    private SaveID_SO _saveSO;
    
    public string SaveableName => name;
    public PuzzleOutputSaveData SaveInfo => _saveData;
    
    public SaveID_SO SaveSO => _saveSO;
    public int SaveID => _saveSO.saveID;

#if UNITY_EDITOR
    public void CreateSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        if (_saveSO == null)
        {
                    #if UNITY_EDITOR
            _saveSO = ScriptableObject.CreateInstance<SaveID_SO>();

            var levelPath = "Assets/LevelSaves/";
            
            AssetDatabase.CreateAsset(_saveSO, levelPath + name + "_ID.asset");
            AssetDatabase.SaveAssets();
            #endif
            
            //EditorUtility.SetDirty(_saveSO);
        }
        
        _saveSO.saveID = ISaveableHelper.GenerateISaveableID(levelSaveableData);
        
        _saveData = new PuzzleOutputSaveData(SaveID, _isSolved);
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(_saveSO);
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
        #endif
        
        Debug.Log($"Created Save Instance for {name}");
    }
#endif
    public void DeleteSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        ISaveableHelper.RemoveExistingID(levelSaveableData, this);
        
        _saveSO.saveID = 0;
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