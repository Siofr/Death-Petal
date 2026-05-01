using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public struct WrongShotPuzzleEvent: IEvent
{
    public Weight weight;
    public WrongShotPuzzleEvent(Weight weightReference) => weight = weightReference;
}

public struct CorrectShotPuzzleEvent : IEvent
{
    public Weight weight;
    public CorrectShotPuzzleEvent(Weight weightReference) => weight = weightReference;
}

public class Weight: PuzzleInputBase
{
    [Header("Weight Fields")] 
    [SerializeField] private Animator _weightModelAnimator;
    
    //[SerializeField] private Bounds _bounds;
    
    [SerializeField] private int _moveSteps;
    [Header("In Seconds")]
    [SerializeField] private float _moveSpeed;
    [Header("In Unity Units")]
    [SerializeField] private float _moveDist;

    [Header("Audio Paths")]
    public EventReference onMoveEventPath;
    
    private Coroutine _moveRoutine;
    private int _routineAccess;
    
    //Properties
    
    private IEnumerator MoveWeightRoutine(bool reset)
    {
        if (PuzzleOutputs[0].IsSolved)
        {
            EventBus<WeightShotEvent>.Raise(new WeightShotEvent(this));
            yield break;
        }
        
        _routineAccess++;
        
        var initPos = transform.position;
        var target = transform.position;
        
        if (_routineAccess > _moveSteps || reset)
        {
            target.y -= _moveDist * (_routineAccess-1);
            _routineAccess = 0;
        }
        else
        {
            target.y += _moveDist;
        }

        for (float i = 0; i < 1; i += Time.deltaTime / _moveSpeed)
        {
            transform.position = Vector3.Lerp(initPos, target, i);
            EventBus<WeightShotEvent>.Raise(new WeightShotEvent(this));
            yield return null;
        }

        transform.position = target;
        
        _moveRoutine = null;
    }

    private void MoveWeight(bool reset)
    {
        if (_moveRoutine != null) return;

        RuntimeManager.PlayOneShot(onMoveEventPath, transform.position);
        _moveRoutine = StartCoroutine(MoveWeightRoutine(reset));
    }
    
    public void Initialize(int moveSteps, float moveSpeed, float moveDist)
    {
        _moveSteps = moveSteps;
        _moveSpeed = moveSpeed;
        _moveDist = moveDist;
    }

    protected override void OnCameraChange(CameraChangeEvent ctx)
    {
        if (PuzzleOutputs[0].IsSolved)
        {
            ToggleAllWeaknesses(false);
            return;
        }
        
        base.OnCameraChange(ctx);
    }

    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness) || _moveRoutine != null) return;

        if (weakness.WeakType != damageType)
        {
            //TODO
            MoveWeight(true);
            EventBus<WrongShotPuzzleEvent>.Raise(new WrongShotPuzzleEvent(this));
        }
        else
        {
            _weightModelAnimator.SetTrigger("Hit");
            MoveWeight(false);
            EventBus<CorrectShotPuzzleEvent>.Raise(new CorrectShotPuzzleEvent(this));
        }
    }

    public override bool CompletionCondition(bool condition, IPuzzleOutput targetOutput)
    {
        ToggleAllWeaknesses(false);
        return base.CompletionCondition(condition, targetOutput);
    }

    // private void Awake()
    // {
    //     base.Awake();
    // }
}