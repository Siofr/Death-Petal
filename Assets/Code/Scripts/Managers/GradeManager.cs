using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Threading.Tasks;
using UnityEditor;

public struct OnLevelEndEvent : IEvent
{
    public Stage stage;

    public OnLevelEndEvent(Stage stage)
    {
        this.stage = stage;
    }
}

public struct OnLevelStartEvent : IEvent
{
    public Stage stage;
    public float time;

    public OnLevelStartEvent(Stage stage,  float time)
    {
        this.stage = stage;
        this.time = time;
    }
}

public struct OnBossKilledEvent : IEvent
{
    
}

public struct DisplayEndUI : IEvent
{
    public Dictionary<string, string> grades;
    public string finalGrade;

    public DisplayEndUI(Dictionary<string, string> grades, string finalGrade)
    {
        this.grades = grades;
        this.finalGrade = finalGrade;
    }
}

public class GradeManager : MonoBehaviour, ISaveable<GradeSaveData>
{
    EventBindings<OnLevelEndEvent> _levelEndEventListener;
    EventBindings<OnLevelStartEvent> _levelStartEventListener;
    EventBindings<UpdateScoreEvent> _updateScoreEventListener;
    EventBindings<OnBossKilledEvent> _onBossKilledEventListener;
    EventBindings<PauseEvent> _pauseEventListener;
    
    public GradeSO[] gradeObjects;
    public Stage currentStage;

    // Time Variables
    public float currentTime;
    public float finalTime;
    public string timeGrade;
    public string scoreGrade;

    // Enemy Variables
    private int _startingEnemies;
    public int enemyCount;
    private string enemyGrade;

    private List<string> grades = new List<string>();
    private string totalLetterGrade;

    public float currentScore;
    
    // Pausing
    private bool _isPaused;
    
    //Saving
    public GradeSaveData _saveData;
    public GradeSaveData SaveInfo => _saveData;

    public SaveID_SO _saveSO;
    
    public SaveID_SO SaveSO => _saveSO;
    
    public int SaveID => _saveSO.saveID;

    public string SaveableName => name;


    // Grade Numerical Value
    private Dictionary<string, int> gradeValue = new Dictionary<string, int>
    {
        {"S", 1 },
        {"A", 2 },
        {"B", 3 },
        {"C", 4 },
        {"D", 5 },
        {"N/A", 0 },
    };

    private Dictionary<int, string> gradeReturn = new Dictionary<int, string>
    {
        {1, "S" },
        {2, "A"},
        {3, "B"},
        {4, "C"},
        {5, "D"},
        {0, "N/A"},
    };

    private void Awake()
    {
        _levelEndEventListener = new EventBindings<OnLevelEndEvent>(OnLevelEnd);
        _levelStartEventListener = new EventBindings<OnLevelStartEvent>(OnLevelStart);
        _updateScoreEventListener = new EventBindings<UpdateScoreEvent>(OnScoreUpdate);
        _onBossKilledEventListener = new EventBindings<OnBossKilledEvent>(OnBossKilled);
        _pauseEventListener = new  EventBindings<PauseEvent>(OnPause);

        _startingEnemies = FindObjectsByType<EnemyBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
    }

    private void OnEnable()
    {
        EventBus<OnLevelEndEvent>.Register(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Register(_levelStartEventListener);
        EventBus<UpdateScoreEvent>.Register(_updateScoreEventListener);
        EventBus<OnBossKilledEvent>.Register(_onBossKilledEventListener);
        EventBus<PauseEvent>.Register(_pauseEventListener);
    }

    private void OnDisable()
    {
        EventBus<OnLevelEndEvent>.Unregister(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Unregister(_levelStartEventListener);
        EventBus<UpdateScoreEvent>.Unregister(_updateScoreEventListener);
        EventBus<OnBossKilledEvent>.Unregister(_onBossKilledEventListener);
        EventBus<PauseEvent>.Unregister(_pauseEventListener);
    }

    private void Update()
    {
        if (_isPaused) return;
        
        currentTime += Time.deltaTime;
    }

    #region Saving

    public async Task CreateSaveInstance(LevelSaveableData_SO levelSaveableData)
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

        _saveData = new GradeSaveData(_saveSO.saveID, currentTime, (int)currentScore, enemyCount, "N/A");
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(_saveSO);
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }

    public async Task DeleteSaveInstance(LevelSaveableData_SO levelSaveableData)
    {
        _saveData = new GradeSaveData();

        if (_saveSO != null && _saveSO.saveID > 0)
        {
            _saveData.id = _saveSO.saveID;
        }
    }

    public void HandleLoadData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;

        _saveData = refData.gradeSaveData;

        _saveData.Load(this);
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        _saveData.Save(this);

        refData.gradeSaveData = _saveData;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
#endif
    }

    #endregion
    
    private void OnPause(PauseEvent ctx)
    {
        _isPaused = ctx.isPaused;
    }
    
    private void OnLevelStart(OnLevelStartEvent ctx)
    {
        // Reset Values for next stage

        currentTime = ctx.time;
        grades.Clear();

        currentStage = ctx.stage;
    }

    private void OnLevelEnd(OnLevelEndEvent ctx)
    {
        Dictionary<string, string> finalGrades = new Dictionary<string, string>();

        // Get Final Time
        finalTime = currentTime;

        // Time Grade
        float bestTime = currentStage.bestTime;

        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (finalTime <= bestTime + gradeObjects[i].timeTaken)
            {
                timeGrade = gradeObjects[i].letterGrade;
                break;
            }
            timeGrade = "D";
        }

        finalGrades.Add(ConvertToTime(finalTime), timeGrade);

        int bestScore = currentStage.bestScore;

        for(int i = 0; i < gradeObjects.Length; i++)
        {
            if (currentScore >= bestScore * gradeObjects[i].percentage)
            {
                scoreGrade = gradeObjects[i].letterGrade;
                break;
            }
            scoreGrade = "D";
        }

        finalGrades.Add(currentScore.ToString(), scoreGrade);

        // Enemies Remaining Grade
        int enemiesRemaining = CheckStageBounds(LayerMask.GetMask("Enemy"));

        enemyGrade = GetEnemyGrade(enemiesRemaining);

        finalGrades.Add(enemiesRemaining.ToString(), enemyGrade);
        totalLetterGrade = GetGradeAverage(finalGrades);

        // Event to display ranking
        EventBus<DisplayEndUI>.Raise(new DisplayEndUI(
            finalGrades,
            totalLetterGrade
            ));
    }

    private void OnBossKilled()
    {
        print("BOSS KILLED");
        EventBus<OnLevelEndEvent>.Raise(new OnLevelEndEvent(currentStage));
    }

    private string ConvertToTime(float time)
    {
        float t = time;

        // float hours = Mathf.Floor(t);
        float minutes = Mathf.Floor(t / 60);
        float seconds = Mathf.Floor(t - minutes * 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private int CheckStageBounds(LayerMask objectLayer)
    {
        Bounds bounds = currentStage.stageBoundary;

        Collider[] overlappingObjects = Physics.OverlapBox(bounds.center, bounds.size, currentStage.transform.rotation, objectLayer);

        var results = 0;

        foreach (var overlappingObject in overlappingObjects)
        {
            var self = overlappingObject.TryGetComponent(out EnemyBase selfEnemy);
            var parent =  overlappingObject.transform.parent.TryGetComponent(out EnemyBase parentEnemy);

            if (self)
            {
                if (!selfEnemy.IsDead)
                {
                    results++;
                    continue;
                }
            }

            if (!parent) continue;

            if (!parentEnemy.IsDead) results++;
        }
        
        return results;
    }

    private string GetGradeAverage(Dictionary<string, string> finalGrades)
    {
        int gradeSum = 0;
        int gradeCount = 0;

        foreach (var entry in  finalGrades)
        {
            if (gradeValue[entry.Value] != 0)
            {
                gradeSum += gradeValue[entry.Value];
                gradeCount++;
            }
        }

        int finalGrade = Mathf.FloorToInt(gradeSum / gradeCount);

        return gradeReturn[finalGrade];
    }

    private string GetEnemyGrade(int enemiesRemaining)
    {
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (enemiesRemaining <= gradeObjects[i].enemiesKilled)
            {
                return gradeObjects[i].letterGrade;
            }
        }

        return "D";
    }

    private void OnScoreUpdate(UpdateScoreEvent ctx)
    {
        currentScore = ctx.score;
    }
}
