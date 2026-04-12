using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CanEditMultipleObjects]
public abstract class PuzzleOutputBase : MonoBehaviour, IPuzzleOutput, ISaveable<PuzzleOutputSaveData>
{
    //Base Fields
    [Header("Base Fields")]
    [SerializeField] private bool _isSolved;
    public Animator animator;
    [FormerlySerializedAs("__camera")]
    [Space]
    [SerializeField] protected CinemachineCamera _camera;
    [SerializeField] protected float _cameraPanTime;
    [SerializeField] protected float _cameraPanSpeed;
    
    //Non-Serialized Fields
    private Coroutine _cameraPanRoutine;
    
    //Events
    private EventBindings<PuzzleSolvedEvent> _puzzleSolvedEventListener;
    private EventBindings<PuzzleResetEvent> _puzzleResetEventListener;
    private EventBindings<LevelLoadedEvent> _levelLoadedEventListener;

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
        _levelLoadedEventListener = new EventBindings<LevelLoadedEvent>(OnLevelLoaded);
    }

    protected void StartPanningCamera(Func<bool> exitCondition)
    {
        if (_camera == null || _cameraPanRoutine != null) return;

        _cameraPanRoutine = StartCoroutine(PanCameraRoutine(exitCondition, _cameraPanSpeed));
    }
    
    protected IEnumerator PanCameraRoutine(Func<bool> exitCondition, float panSpeed)
    {
        print("ended panning Camera");
        
        EventBus<CameraChangeEvent>.DisableEvent();

        if (InputHandler.Instance != null) InputHandler.Instance.enabled = false;
        
        if (_cameraPanRoutine != null || _camera == null) yield break;

        var brain = FindAnyObjectByType<CinemachineBrain>();
        
        if (brain == null) yield break;

        var currentCam = _camera.GetComponent<ICinemachineCamera>();
        var lastCam = brain.ActiveVirtualCamera;
        
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, panSpeed);
        
        _camera.gameObject.SetActive(true);

        var isComplete = false;

        while (!isComplete)
        {
            isComplete = exitCondition.Invoke();
            
            yield return null;
        }
        
        yield return new WaitForSeconds(_cameraPanTime);
        
        _camera.gameObject.SetActive(false);

        EventBus<CameraChangeEvent>.EnableEvent();
        
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0);
        
        if (InputHandler.Instance != null) InputHandler.Instance.enabled = true;
        
        _cameraPanRoutine = null;
        
        print("started panning Camera");
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
        EventBus<LevelLoadedEvent>.Register(_levelLoadedEventListener);
    }

    public virtual void OnDisable()
    {
        EventBus<PuzzleSolvedEvent>.Unregister(_puzzleSolvedEventListener);
        EventBus<PuzzleResetEvent>.Unregister(_puzzleResetEventListener);
        EventBus<LevelLoadedEvent>.Unregister(_levelLoadedEventListener);
    }
    
    public virtual void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if ((PuzzleOutputBase)context.puzzleOutput != this) return;
        
        RuntimeManager.PlayOneShot(onCompletionEventPath, transform.position);
        if(animator != null) animator.SetBool(Animator.StringToHash("IsSolved"), true);
        IsSolved = true;
    }

    private void OnLevelLoaded(LevelLoadedEvent levelLoadedEvent)
    {
        if (_saveData.isSolved)
        {
            IsSolved = true;
            EventBus<PuzzleSolvedEvent>.Raise(new PuzzleSolvedEvent(this));
        }
    }

    public virtual void OnPuzzleReset(PuzzleResetEvent context)
    {
        if ((PuzzleOutputBase)context.puzzleOutput != this) return;
        
        if(animator != null) animator.SetBool(Animator.StringToHash("IsSolved"), false);
        IsSolved = false;
    }
    
    //Saving Stuff
    
    [SerializeField] private PuzzleOutputSaveData _saveData;
    [SerializeField] private SaveID_SO _saveSO;
    
    public string SaveableName => name;
    public PuzzleOutputSaveData SaveInfo => _saveData;
    
    public SaveID_SO SaveSO => _saveSO;
    public int SaveID => _saveSO.saveID;
    
    public void CreateSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        if (_saveSO == null)
        {
#if UNITY_EDITOR
            _saveSO = ScriptableObject.CreateInstance<SaveID_SO>();
            
            var levelPath = "Assets/LevelSaves/";
            var fileName = name;
            
            if (ISaveableHelper.existingNames.ContainsKey(name))
            {
                fileName += ISaveableHelper.existingNames[name] + 1;

                ISaveableHelper.existingNames[name]++;
            }
            else
            {
                ISaveableHelper.existingNames.Add(fileName, 0);
            }
            AssetDatabase.SaveAssets();
            
            AssetDatabase.CreateAsset(_saveSO, levelPath + fileName + "_ID.asset");
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
    
    public void DeleteSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        _saveData = new PuzzleOutputSaveData();
        
        if (_saveSO == null) return;
        
        ISaveableHelper.RemoveExistingID(levelSaveableData, this);
        _saveSO.saveID = 0;
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        _saveData.Save(IsSolved);
        
        for (var i = 0; i < refData.puzzleOutputSaveData.Count; i++)
        {
            if (refData.puzzleOutputSaveData[i].id != SaveID) continue;
            
            refData.puzzleOutputSaveData[i] = _saveData;
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
            
            return;
        }
        
        refData.puzzleOutputSaveData.Add(_saveData);
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
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
        }
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }
}