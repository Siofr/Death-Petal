using System.Collections.Generic;
using UnityEngine;

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
    
    //Properties
    public List<Weight> Weights => _weights;
    
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
    
    private void Start()
    {
        InitializeWeights();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weight"))
        {
            Debug.Log("Checking Weights");
            
            if(!_weightColliders.ContainsKey(other) && other.TryGetComponent(out Weight weight))
            {
                _weightColliders.Add(other, weight);
            }
            
            CheckCompletionConditions();
        }

        Debug.Log(other.gameObject.name);
        Debug.Log($"Number of Weight Collider {_weightColliders.Count}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weight"))
        {
            _weightColliders.Remove(other);
            
            CheckCompletionConditions();
        }
    }
}