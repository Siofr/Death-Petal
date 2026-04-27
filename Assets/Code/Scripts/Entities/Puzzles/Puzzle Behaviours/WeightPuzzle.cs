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
    private Dictionary<Weight, Collider> _weightColliders = new Dictionary<Weight, Collider>();
    public Collider[] puzzleColliders;
    
    //Properties
    public List<Weight> Weights => _weights;
    
    //Events
    public EventBindings<WeightShotEvent> _weightShotEventListener;

    protected override void Start()
    {
        base.Start();
        //InitializeWeights();

        foreach (var output in PuzzleOutputs)
        {
            print($"{name} , PuzzleOutput: {output}");
        }
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

    // private void InitializeWeights()
    // {
    // }
    
    private void CheckCompletionConditions()
    {
        var conditionIndex = 0;
        
        foreach(var weight in _weights)
        {
            if (_weightColliders.ContainsKey(weight))
            {
                conditionIndex++;
                
                foreach(var output in weight.PuzzleOutputs)
                {
                    weight.CompletionCondition(true, output);
                }
            }
        }

        var overallCondition = conditionIndex >= _weights.Count;

        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(overallCondition, output);
        }
    }
    

    private void OnWeightShot(WeightShotEvent context)
    {
        if (puzzleColliders.Length < 1 || _weightColliders == null) return;

        var isWeightCompleted = false;
        
        foreach (var pCollider in puzzleColliders)
        {
            if (!pCollider.bounds.Intersects(context.collider.bounds)) continue;

            if (!_weightColliders.ContainsKey(context.weight))
            {
                _weightColliders.Add(context.weight, context.collider);
                break;
            }
        }
        
        CheckCompletionConditions();
    }
}