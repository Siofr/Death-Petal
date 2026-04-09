using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public abstract class EntityBase : MonoBehaviour, IEntity, ISaveable<EntitySaveData>
{
    //Non-Serializable Fields
    private List<Weakness> _weaknesses = new List<Weakness>();
    
    //Properties
    public List<Weakness> Weaknesses => _weaknesses;

    public void RemoveNulledWeakness(Weakness weakness)
    {
        _weaknesses.Remove(weakness);
    }
    
    [FormerlySerializedAs("_sequentialWeaknesses")]
    [Header("Entity Sequential Fields")]
    [SerializeField] protected bool __sequentialWeaknesses;
    public List<WeakTypes> defaultWeaknessTypes;
    
    //Events
    private EventBindings<CameraChangeEvent> _onCameraChange;
    
    protected virtual void Awake()
    {
        // InitialiseWeaknesses();
    }

    protected virtual void Start()
    {
        InitialiseWeaknesses();
    }

    protected virtual void OnEnable()
    {
        _onCameraChange = new EventBindings<CameraChangeEvent>(OnCameraChange);
        EventBus<CameraChangeEvent>.Register(_onCameraChange);
    }

    protected virtual void OnDisable()
    {
        EventBus<CameraChangeEvent>.Unregister(_onCameraChange);
    }
    
    public virtual void InitialiseWeaknesses()
    {
        if (_weaknesses == null) return;
        
        var weaknesses = GetComponentsInChildren<Weakness>();

        if (weaknesses == null) return;
        
        _weaknesses.Clear();
        
        foreach (var weakness in weaknesses)
        {
            //weakness.Initialise();
            _weaknesses.Add(weakness);
        }
        
        if (!__sequentialWeaknesses) return;
        
        defaultWeaknessTypes.Clear();
        
        if (Weaknesses.Count > 0)
        {
            for (int i = 0; i < Weaknesses.Count; i++)
            {
                defaultWeaknessTypes.Add(Weaknesses[i].WeakType);

                if (i == 0) continue;
                
                //Weaknesses[i].SetWeakType(WeakTypes.PLAYER);
                Weaknesses[i].Toggle(false);
            }
        }

        for (int i = _weaknesses.Count - 1; i >= 0; i--)
        {
            if (Weaknesses[i].WeakType == WeakTypes.NONE)
            {
                Weaknesses[i].StartDelayDestroy();
                Weaknesses.RemoveAt(i);
            }
        }
    }

    public void ToggleAllWeaknesses(bool toggle)
    {
        foreach (var weakness in _weaknesses)
        {
            weakness.Toggle(toggle);
        }
    }
    
    public void ToggleAllWeaknessIcons(bool toggle)
    {
        foreach(var weakness in Weaknesses) weakness.ToggleIcon(toggle);
    }
    
    public void IntitialiseWeaknesses(List<Weakness> weaknesses)
    {
        _weaknesses = weaknesses;
    }
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
    
    //Saving
    [SerializeField] private EntitySaveData _saveData;
    [SerializeField] private SaveID_SO _saveSO;

    private void OnCameraChange(CameraChangeEvent ctx)
    {
        if(ctx.entities.Contains(this)) Weaknesses[0].Toggle(true);
        else ToggleAllWeaknessIcons(false);
    }
    
    public string SaveableName => name;
    public EntitySaveData SaveInfo =>  _saveData;
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
            
            AssetDatabase.CreateAsset(_saveSO, levelPath + fileName + "_ID.asset");
            EditorUtility.SetDirty(_saveSO);
             #endif
        }
        
        _saveSO.saveID = ISaveableHelper.GenerateISaveableID(levelSaveableData);
        InitialiseWeaknesses();
        
        var health = new List<WeaknessSaveData>();
        for (int i = 0; i < Weaknesses.Count; i++)
        {
            health.Add(new WeaknessSaveData(Weaknesses[i].WeaknessIconTransform.position, (int)Weaknesses[i].WeakType));
        }
        
        _saveData = new EntitySaveData(SaveID, transform.position, health);

 #if UNITY_EDITOR
            EditorUtility.SetDirty(_saveSO);
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }

    public void DeleteSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        _saveData = new EntitySaveData();
        _saveSO = null;
        
        if (_saveSO == null) return;
        
        ISaveableHelper.RemoveExistingID(levelSaveableData, this);
        _saveSO.saveID = 0;
    }
    
    public void HandleLoadData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;

        foreach (var data in refData.entitySaveData)
        {
            if (data.id != SaveID) continue;
            
            InitialiseWeaknesses();
            
            _saveData = data;
            _saveData.Load(transform, ref _weaknesses);
        }
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif

        InitialiseWeaknesses();
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        InitialiseWeaknesses();
        
        _saveData.Save(transform.position, Weaknesses);

        for (var i = 0; i < refData.entitySaveData.Count; i++)
        {
            if (refData.entitySaveData[i].id != SaveID) continue;

            refData.entitySaveData[i] = _saveData;
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
            
            return;
        }
        
        refData.entitySaveData.Add(_saveData);
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }
}