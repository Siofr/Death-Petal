using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct OnLevelEndEvent : IEvent
{

}

public struct OnLevelStartEvent : IEvent
{

}

public class GradeManager : MonoBehaviour
{
    EventBindings<OnLevelEndEvent> _levelEndEventListener;
    EventBindings<OnLevelStartEvent> _levelStartEventListener;

    public GradeSO[] gradeObjects;

    // Time Variables
    private float bestTime;
    private float currentTime;
    private float finalTime;
    private string timeGrade;

    // Damage Variables
    EventBindings<PlayerDamagedEvent> _playerDamagedEventListener;
    private int damageTaken;
    private string damageGrade;

    // Enemy Variables
    private int enemiesRemaining;
    private string enemyGrade;

    // Soul Urns Variables
    private int urnsRemaining;
    private string urnGrade;

    // Petal Variables
    private int petalsRemaining;
    private string petalGrade;

    // Bullets Reflected
    EventBindings<WrongShotEvent> _wrongShotEventListener;
    private int bulletReflections;
    private string reflectionGrade;

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

    private void OnLevelStart()
    {
        currentTime = 0;
    }

    private void OnLevelEnd()
    {
        // Get Final Time
        finalTime = currentTime;

        // Find how many enemies remain in level
        enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // Find how many petals remain in level
        petalsRemaining = GameObject.FindGameObjectsWithTag("Petal").Length;

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

        // Time Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (finalTime <= bestTime + gradeObjects[i].timeTaken * 60)
            {
                timeGrade = gradeObjects[i].letterGrade;
                break;
            }
            timeGrade = "D";
        }

        // Enemies Remaining Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (enemiesRemaining <= gradeObjects[i].enemiesKilled)
            {
                enemyGrade = gradeObjects[i].letterGrade;
                break;
            }
            enemyGrade = "D";
        }

        // Petals Remaining Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (petalsRemaining <= gradeObjects[i].petalsRemaining)
            {
                petalGrade = gradeObjects[i].letterGrade;
                break;
            }
            petalGrade = "D";
        }

        // Urn Grade
        for (int i = 0; i < gradeObjects.Length; i++)
        {
            if (urnsRemaining <= gradeObjects[i].urnsBroken)
            {
                urnGrade = gradeObjects[i].letterGrade;
                break;
            }
            urnGrade = "D";
        }

        int totalGrade = gradeValue[petalGrade] + gradeValue[urnGrade] + gradeValue[enemyGrade] + gradeValue[damageGrade] + gradeValue[reflectionGrade] + gradeValue[timeGrade];
        int finalGrade = Mathf.FloorToInt(totalGrade / 6);
        Debug.Log(finalGrade);
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
