using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weight: MonoBehaviour, IEntity
{
    [Header("Weight Fields")] 
    [SerializeField] private GameObject _linkedOutputObject;
    //[SerializeField] private Bounds _bounds;
    
    [SerializeField] private int _moveSteps;
    [Header("In Seconds")]
    [SerializeField] private float _moveSpeed;
    [Header("In Unity Units")]
    [SerializeField] private float _moveDist;
    
    //Non-Serializable Fields
    private List<Weakness> _weaknesses = new List<Weakness>();
    private IPuzzleOutput _output;

    private Coroutine _moveRoutine;
    private int _routineAccess;
    
    //Properties
    public IPuzzleOutput LinkedOutput => _output;
    public List<Weakness> Weaknesses => _weaknesses;

    private IEnumerator MoveWeightRoutine(bool reset)
    {
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

            yield return null;
        }

        transform.position = target;
        
        _moveRoutine = null;
    }

    private void MoveWeight(bool reset)
    {
        if (_moveRoutine != null) return;
        
        _moveRoutine = StartCoroutine(MoveWeightRoutine(reset));
    }
    
    public void Initialize(int moveSteps, float moveSpeed, float moveDist)
    {
        _moveSteps = moveSteps;
        _moveSpeed = moveSpeed;
        _moveDist = moveDist;
    }
    
    public void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness) || _moveRoutine != null) return;

        if (weakness.WeakType != damageType)
        {
            //TODO
            MoveWeight(true);
        }
        else
        {
            MoveWeight(false);
        }
    }
    
    private void Awake()
    {
        if(_linkedOutputObject != null) _linkedOutputObject.TryGetComponent(out _output);
    }
}