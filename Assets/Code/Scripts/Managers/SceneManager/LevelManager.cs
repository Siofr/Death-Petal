using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct LevelSaveEvent : IEvent { }

public struct LevelLoadEvent : IEvent
{
    public bool isDefault;

    public LevelLoadEvent(bool isDefault = true)
    {
        this.isDefault = isDefault;
    }
}



public class LevelManager : MonoBehaviour
{
    [Header("LevelManager Fields, Attach Component to Level Prefab")]
    public LevelSaveableData_SO saveableData;
    public LevelSaveData levelSaveData;
    public bool playIntro = false;
    
    public List<GameObject> saveableGO = new List<GameObject>();
    
    public void SaveLevelData(bool isBaking = false)
    {
        var temp = new LevelSaveData(SceneManager.GetActiveScene().name);
        
        var tempSaveables = FindSaveables().Result;

        if (tempSaveables.Count < 1)
        {
            Debug.Log("No Saveable Objects in Level");
            return;
        }

        saveableGO = tempSaveables;
        
        var saveables = GetSaveables();
        var tempIDs = new List<int>();
        
        foreach (var saveable in saveables)
        {
            tempIDs.Add(saveable.SaveID);
            //saveable.HandleSaveData(ref temp);
        }

        temp.saveableID = tempIDs;
        
        foreach (var saveable in saveables)
        {
            saveable.HandleSaveData(ref temp);
        }

        levelSaveData = temp;
        
#if UNITY_EDITOR
        
        EditorUtility.SetDirty(saveableData);
        EditorUtility.SetDirty(this);

        PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject); 
        
#endif
        
        SaveSystem.SaveLevelData(temp);
        
        Debug.Log("Saved Level Data");
    }

    public void LoadLevelData(bool isDefault = false)
    {
        if (isDefault)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            var tempName = SceneManager.GetActiveScene().name;
        
            var temp = SaveSystem.GetLevelData(tempName);
        
            if (temp.levelName != SceneManager.GetActiveScene().name && temp.levelName != SceneManager.GetActiveScene().name + " Default") return;

            levelSaveData = temp;

            saveableGO = FindSaveables().Result;
            var tempSaveables = GetSaveablesWithObjects();

            foreach (var tempSaveable in tempSaveables)
            {
                if (temp.saveableID.Contains(tempSaveable.Value.SaveID))
                {
                    
                    continue;
                }
                
                tempSaveable.Key.SetActive(false);
            }
            
            foreach (var saveable in tempSaveables.Values)
            {
                saveable.HandleLoadData(ref levelSaveData);
            }
        }
        
        Debug.Log("Loaded Level Data");
        
        EntityHelper.UnlockAllInputs();
        
        EventBus<LevelLoadedEvent>.Raise(new LevelLoadedEvent());
    }
    
    public async Task<List<GameObject>> FindSaveables()
    {
        if (saveableData == null)
        {
            Debug.Log("Please Create a Level Saveable Data Sciptable Object Asset And Attach To Component");
        }
        
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var tempSaveables = new List<ISaveable>();
        var saveableObjects = new List<GameObject>();
            
        foreach (var gameObj in sceneObjects)
        {
            if (!gameObj.TryGetComponent(out ISaveable saveable)) continue;
            saveableObjects.Add(gameObj);
            tempSaveables.Add(saveable);
        }
        
        if(tempSaveables.Count < 1) Debug.Log("No Saveable Objects in Level");

        await InitialiseSaveables(tempSaveables);

        return saveableObjects;
    }

    private async Task InitialiseSaveables(List<ISaveable> saveables)
    {
        foreach (var saveable in saveables)
        {
            if (saveable.SaveSO == null || saveable.SaveSO.saveID < 1)
            {
                //Debug.Log(saveable.SaveSO.saveID);
                await saveable.CreateSaveInstance(saveableData);
            }
        }
    }
    
    public List<ISaveable> GetSaveables()
    {
        var temp = new List<ISaveable>();

        foreach (var go in saveableGO)
        {
            temp.Add(go.GetComponent<ISaveable>());
        }
            
        return temp;
    }

    public Dictionary<GameObject, ISaveable> GetSaveablesWithObjects()
    {
        var temp = new Dictionary<GameObject, ISaveable>();

        foreach (var go in saveableGO)
        {
            temp.Add(go, go.GetComponent<ISaveable>());
        }

        return temp;
    }
    
    public void ClearData()
    {
        var saveables = GetSaveables();
        
        foreach (var saveable in saveables)
        {
            saveable.DeleteSaveInstance(saveableData);
        }
        
        levelSaveData =  new LevelSaveData(SceneManager.GetActiveScene().name);
        saveableGO.Clear();
        
        SaveSystem.SaveGameData();
        
        Debug.Log("Cleared Saveable Objects");
    }
    
    private void OnLoadRequest(LevelLoadEvent ctx)
    {
        isLoadingDefault = ctx.isDefault;
        
        //LoadLevelData(ctx.isDefault);
    }

    private void OnSaveRequest(LevelSaveEvent ctx)
    {
        SaveLevelData();
    }
    
    private EventBindings<LevelLoadEvent> _loadRequestListener;
    private EventBindings<LevelSaveEvent> _saveRequestListener;
    
    private void OnEnable()
    {
        _loadRequestListener = new EventBindings<LevelLoadEvent>(OnLoadRequest);
        _saveRequestListener = new EventBindings<LevelSaveEvent>(OnSaveRequest);
        
        EventBus<LevelLoadEvent>.Register(_loadRequestListener);
        EventBus<LevelSaveEvent>.Register(_saveRequestListener);
    }

    private void OnDisable()
    {
        EventBus<LevelLoadEvent>.Unregister(_loadRequestListener);
        EventBus<LevelSaveEvent>.Unregister(_saveRequestListener);
    }

    static public bool isLoadingDefault = true;
    
    public void Start()
    {
        //if(saveables.Count < 1) saveables = FindSaveables();
        //LoadLevelData();

        if (!isLoadingDefault)
        {
            LoadLevelData();
            
            EntityHelper.UnlockAllInputs();
        }
        
        EventBus<SetTransitionEvent>.Raise( new SetTransitionEvent(false, true));
        if(playIntro)
            StartCoroutine(PlayIntro());

    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForFixedUpdate();
        EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent("L1Intro"));
    }


}
