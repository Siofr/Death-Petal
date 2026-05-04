using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PuzzleTriggerInput: PuzzleInputBase
{
    [Space]
    [Header("Puzzle Trigger Fields")]
    [SerializeField] private bool _isOneTimeUse;

    private bool _isUsed;
    private BoxCollider _collider;

    private void OnDrawGizmos()
    {
        if(_collider == null) _collider = GetComponent<BoxCollider>();

        var redClear = new Color(1, 0, 0, .25f);
        
        Gizmos.color = redClear;
        Gizmos.DrawCube(transform.position+_collider.center, _collider.size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isOneTimeUse && _isUsed) return;
        
        if(PuzzleOutputs[0].IsSolved) return;
        
        if (other.CompareTag("Player")) _isUsed = true;
        
        foreach (IPuzzleOutput puzzleOutput in PuzzleOutputs)
        {
            puzzleOutput.OnPuzzleBoundsEntered();
        }
        
        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(other.CompareTag("Player"), output);
        }
    }
}