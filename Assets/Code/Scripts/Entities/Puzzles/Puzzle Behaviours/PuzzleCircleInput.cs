using UnityEngine;

public class PuzzleCircleInput: PuzzleInputBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var output in PuzzleOutputs)
            {
                CompletionCondition(true, output);
            }
        }
    }
}