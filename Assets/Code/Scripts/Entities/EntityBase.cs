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
                
                Weaknesses[i].SetWeakType(WeakTypes.PLAYER);
                Weaknesses[i].ToggleHitbox(false);
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
        if(ctx.entities.Contains(this)) ToggleAllWeaknessIcons(true);
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
            
            AssetDatabase.CreateAsset(_saveSO, levelPath + name + "_ID.asset");
            EditorUtility.SetDirty(_saveSO);
             #endif
        }
        
        _saveSO.saveID = ISaveableHelper.GenerateISaveableID(levelSaveableData);
        InitialiseWeaknesses();
        
        var health = new List<int>();
        Weaknesses.ForEach(x=>health.Add((int)x.WeakType));
        
        _saveData = new EntitySaveData(SaveID, transform.position, health);

 #if UNITY_EDITOR
            EditorUtility.SetDirty(_saveSO);
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }

    public void DeleteSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        ISaveableHelper.RemoveExistingID(levelSaveableData, this);

        _saveSO.saveID = 0;
        _saveData = new EntitySaveData();
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
            
            if(data.health.Count > 0) InitialiseWeaknesses();
            else
            {
                gameObject.SetActive(false);
            }
            return;
        }
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        _saveData.Save(transform.position, Weaknesses);

        for (var i = 0; i < refData.entitySaveData.Count; i++)
        {
            if (refData.entitySaveData[i].id != SaveID) continue;

            refData.entitySaveData[i] = _saveData;
            return;
        }
        
        refData.entitySaveData.Add(_saveData);
    }
}