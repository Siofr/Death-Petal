using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;

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

    public OnLevelStartEvent(Stage stage)
    {
        this.stage = stage;
    }
}

public struct DisplayEndUI : IEvent
{
    public Dictionary<float, string> grades;
    public string finalGrade;

    public DisplayEndUI(Dictionary<float, string> grades, string finalGrade)
    {
        this.grades = grades;
        this.finalGrade = finalGrade;
    }
}

public class GradeManager : MonoBehaviour
{
    EventBindings<OnLevelEndEvent> _levelEndEventListener;
    EventBindings<OnLevelStartEvent> _levelStartEventListener;
    EventBindings<UpdateScoreEvent> _updateScoreEventListener;

    public GradeSO[] gradeObjects;
    private Stage currentStage;

    // Time Variables
    private float currentTime;
    private float finalTime;
    private string timeGrade;
    private string scoreGrade;

    // Enemy Variables
    private int enemyCount;
    private string enemyGrade;

    private List<string> grades = new List<string>();
    private string totalLetterGrade;

    private float currentScore;

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
    }

    private void OnEnable()
    {
        EventBus<OnLevelEndEvent>.Register(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Register(_levelStartEventListener);
        EventBus<UpdateScoreEvent>.Register(_updateScoreEventListener);
    }

    private void OnDisable()
    {
        EventBus<OnLevelEndEvent>.Unregister(_levelEndEventListener);
        EventBus<OnLevelStartEvent>.Unregister(_levelStartEventListener);
        EventBus<UpdateScoreEvent>.Unregister(_updateScoreEventListener);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }

    private void OnLevelStart(OnLevelStartEvent ctx)
    {
        // Reset Values for next stage

        currentTime = 0;
        grades.Clear();

        currentStage = ctx.stage;

        enemyCount = CheckStageBounds(LayerMask.GetMask("Enemy"));
    }

    private void OnLevelEnd(OnLevelEndEvent ctx)
    {
        Dictionary<float, string> finalGrades = new Dictionary<float, string>();

        // Get Final Time
        finalTime = currentTime;

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

        finalGrades.Add(finalTime, timeGrade);

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

        finalGrades.Add(currentScore, scoreGrade);

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

        finalGrades.Add(enemiesRemaining, enemyGrade);
        totalLetterGrade = GetGradeAverage(finalGrades);

        // Event to display ranking
        EventBus<DisplayEndUI>.Raise(new DisplayEndUI(
            finalGrades,
            totalLetterGrade
            ));
    }

    private int CheckStageBounds(LayerMask objectLayer)
    {
        Bounds bounds = currentStage.stageBoundary;

        Collider[] overlappingObjects = Physics.OverlapBox(bounds.center, bounds.size, currentStage.transform.rotation, objectLayer);
        return overlappingObjects.Length;
    }

    private string GetGradeAverage(Dictionary<float, string> finalGrades)
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
