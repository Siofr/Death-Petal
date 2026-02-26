using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;

public struct OnLevelEndEvent : IEvent
{

}

public struct OnLevelStartEvent : IEvent
{
    public Stage stage;

    public OnLevelStartEvent(Stage stage)
    {
        this.stage = stage;
    }
}

public struct DisplayEndUI : IEvent
{
    public string finalGrade;
    public string enemyGrade;
    public string urnGrade;
    public string puzzleGrade;
    public string petalGrade;
    public string reflectionGrade;
    public string damageGrade;
    public string timeGrade;

    public DisplayEndUI(string finalGrade, string enemyGrade, string urnGrade, string puzzleGrade, string petalGrade, string reflectionGrade, string damageGrade, string timeGrade)
    {
        this.finalGrade = finalGrade;
        this.enemyGrade = enemyGrade;
        this.urnGrade = urnGrade;
        this.puzzleGrade = puzzleGrade;
        this.petalGrade = petalGrade;
        this.reflectionGrade = reflectionGrade;
        this.damageGrade = damageGrade;
        this.timeGrade = timeGrade;
    }
}

public class GradeManager : MonoBehaviour
{
    EventBindings<OnLevelEndEvent> _levelEndEventListener;
    EventBindings<OnLevelStartEvent> _levelStartEventListener;

    public GradeSO[] gradeObjects;
    private Stage currentStage;

    // Time Variables
    private float currentTime;
    private float finalTime;
    private string timeGrade;

    // Damage Variables
    EventBindings<PlayerDamagedEvent> _playerDamagedEventListener;
    private int damageTaken;
    private string damageGrade;

    // Enemy Variables
    private int enemyCount;
    private string enemyGrade;

    // Soul Urns Variables
    private int urnCount;
    private string urnGrade;

    // Petal Variables
    private int petalsRemaining;
    private string petalGrade;

    // Puzzle Variables
    private int puzzlesCompleted;
    private int puzzleCount;
    private string puzzleGrade;

    // Bullets Reflected
    EventBindings<WrongShotEvent> _wrongShotEventListener;
    private int bulletReflections;
    private string reflectionGrade;

    private List<string> grades = new List<string>();
    private string totalLetterGrade;

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
        _playerDamagedEventListener = new EventBindings<PlayerDamagedEvent>(OnDamageTaken);
        _wrongShotEventListener = new EventBindings<WrongShotEvent>(OnReflection);
        _levelEndEventListener = new EventBindings<OnLevelEndEvent>(OnLevelEnd);
        _levelStartEventListener = new EventBindings<OnLevelStartEvent>(OnLevelStart);
    }

    private void OnEnable()
    {
        EventBus<PlayerDamagedEvent>.Register(_playerDamagedEventListener);
        EventBus<WrongShotEvent>.Register(_wrongShotEventListener);
        EventBus<OnLevelEndEvent>.Register(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Register(_levelStartEventListener);
    }

    private void OnDisable()
    {
        EventBus<PlayerDamagedEvent>.Unregister(_playerDamagedEventListener);
        EventBus<WrongShotEvent>.Unregister(_wrongShotEventListener);
        EventBus<OnLevelEndEvent>.Unregister(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Unregister(_levelStartEventListener);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }

    private void OnLevelStart(OnLevelStartEvent ctx)
    {
        // Reset Values for next stage

        currentTime = 0;
        bulletReflections = 0;
        damageTaken = 0;
        grades.Clear();

        currentStage = ctx.stage;

        enemyCount = CheckStageBounds(LayerMask.GetMask("Enemy"));
        urnCount = CheckStageBounds(LayerMask.GetMask("Urn"));
        puzzleCount = currentStage.puzzleCount;
    }

    private void OnLevelEnd(OnLevelEndEvent ctx)
    {
        // Get Final Time
        finalTime = currentTime;

        // Damage Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (damageTaken <= gradeObjects[i].damageTaken)
            {
                damageGrade = gradeObjects[i].letterGrade;
                break;
            }
            damageGrade = "D";
        }

        grades.Add(damageGrade);

        // Bullets Reflected Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (bulletReflections <= gradeObjects[i].bulletsReflected)
            {
                reflectionGrade = gradeObjects[i].letterGrade;
                break;
            }
            reflectionGrade = "D";
        }

        grades.Add(reflectionGrade);

        // Time Grade
        float bestTime = currentStage.bestTime;

        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (finalTime <= bestTime + gradeObjects[i].timeTaken * 60)
            {
                timeGrade = gradeObjects[i].letterGrade;
                break;
            }
            timeGrade = "D";
        }

        grades.Add(timeGrade);

        // Enemies Remaining Grade
        int enemiesRemaining = CheckStageBounds(LayerMask.GetMask("Enemy"));

        if (enemyCount <= 0)
        {
            enemyGrade = "N/A";
        }
        else
        {
            enemyGrade = GetEnemyGrade(enemiesRemaining);
        }

        grades.Add(enemyGrade);

        // Petals Remaining Grade
        int petalsRemaining = CheckStageBounds(LayerMask.GetMask("Petal"));

        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (petalsRemaining <= gradeObjects[i].petalsRemaining)
            {
                petalGrade = gradeObjects[i].letterGrade;
                break;
            }
            petalGrade = "D";
        }

        grades.Add(petalGrade);

        // Puzzle Grade
        if (puzzleCount <= 0)
        {
            puzzleGrade = "N/A";
        }
        else
        {
            GetPuzzleGrade();
        }

        grades.Add(puzzleGrade);

        // Urn Grade
        int urnsRemaining = CheckStageBounds(LayerMask.GetMask("Urn"));

        if (urnCount <= 0)
        {
            urnGrade = "N/A";
        }
        else
        {
            urnGrade = GetUrnGrade(urnsRemaining);
        }

        grades.Add(urnGrade);

        totalLetterGrade = GetGradeAverage();

        // Event to display ranking
        EventBus<DisplayEndUI>.Raise(new DisplayEndUI(
            totalLetterGrade,
            enemyGrade,
            urnGrade,
            puzzleGrade,
            petalGrade,
            reflectionGrade,
            damageGrade,
            timeGrade
            ));
    }

    private int CheckStageBounds(LayerMask objectLayer)
    {
        Bounds bounds = currentStage.stageBoundary;

        Collider[] overlappingObjects = Physics.OverlapBox(bounds.center, bounds.size, currentStage.transform.rotation, objectLayer);
        return overlappingObjects.Length;
    }

    private string GetGradeAverage()
    {
        int gradeSum = 0;
        int gradeCount = 0;

        for (int i = 0; i < grades.Count - 1; i++)
        {
            if (gradeValue[grades[i]] != 0)
            {
                gradeSum += gradeValue[grades[i]];
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

    private string GetPuzzleGrade()
    {
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (puzzleCount - puzzlesCompleted <= gradeObjects[i].petalsRemaining)
            {
                return gradeObjects[i].letterGrade;
            }
        }

        return "D";
    }

    private string GetUrnGrade(int urnsRemaining)
    {
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (urnsRemaining <= gradeObjects[i].urnsBroken)
            {
                return gradeObjects[i].letterGrade;
            }
        }
        return "D";
    }

    private void OnPuzzleCompleted()
    {
        puzzlesCompleted++;
    }

    private void OnDamageTaken()
    {
        damageTaken++;
    }

    private void OnReflection()
    {
        bulletReflections++;
    }
}
