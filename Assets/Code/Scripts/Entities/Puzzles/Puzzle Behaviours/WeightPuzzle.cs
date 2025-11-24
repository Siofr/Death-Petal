using System.Collections.Generic;
using UnityEngine;

public struct WeightShotEvent: IEvent
{
    public Weight weight;
    public Collider collider;
    
    public WeightShotEvent(Weight value)
    {
        weight = value;
        collider = weight.GetComponent<Collider>();
    }
}

public class WeightPuzzle : PuzzleInputBase
{
    [Header("Weight Puzzle Fields")] 
    [SerializeField] private List<Weight> _weights = new List<Weight>();
    [SerializeField] private int _weightSteps;
    [Header("In Seconds")]
    [SerializeField] private float _weightSpeed;
    [Header("In Unity Units")]
    [SerializeField] private float _weightDist;
    
    //Non-Serializable Fields   
    private Dictionary<Collider, Weight> _weightColliders = new Dictionary<Collider, Weight>();
    private Collider _puzzleCollider;
    
    //Properties
    public List<Weight> Weights => _weights;
    
    //Events
    public EventBindings<WeightShotEvent> _weightShotEventListener;

    protected override void Awake()
    {
        base.Awake();
        InitializeWeights();
        _puzzleCollider = TryGetComponent(out Collider temp) ? temp : null;
    }

    private void OnEnable()
    {
        _weightShotEventListener = new EventBindings<WeightShotEvent>(OnWeightShot);
        
        EventBus<WeightShotEvent>.Register(_weightShotEventListener);
    }

    private void OnDisable()
    {
        EventBus<WeightShotEvent>.Unregister(_weightShotEventListener);
    }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
    }

    private void InitializeWeights()
    {
        List<Weight> nullWeights = new List<Weight>();
        
        foreach (var weight in _weights)
        {
            if (weight.LinkedOutput != null && PuzzleOutputs.Contains(weight.LinkedOutput))
            {
                PuzzleOutputs.Remove(weight.LinkedOutput);
            }
            else
            {
                nullWeights.Add(weight);
            }
        }
        
        foreach (var weight in nullWeights)
            _weights.Remove(weight);

        foreach (var weight in _weights)
        {
            weight.Initialize(_weightSteps, _weightSpeed, _weightDist);
        }
    }
    
    private void CheckCompletionConditions()
    {
        var conditionIndex = 0;
        
        foreach(var weight in _weights)
        {
            conditionIndex = CompletionCondition(_weightColliders.ContainsValue(weight), weight.LinkedOutput) ? conditionIndex+1 : conditionIndex;
        }

        var overallCondition = conditionIndex >= _weights.Count;

        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(overallCondition, output);
        }
    }
    

    private void OnWeightShot(WeightShotEvent context)
    {
        if (_puzzleCollider.bounds.Intersects(context.collider.bounds))
        {
            if (!_weightColliders.ContainsKey(context.collider))
            {
                _weightColliders.Add(context.collider, context.weight);       
            }
        }
        else
        {
            _weightColliders.Remove(context.collider);
        }
        
        CheckCompletionConditions();
    }
}