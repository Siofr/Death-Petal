using System;
using UnityEngine;

public interface IPuzzleInput: IEntity
{
    IPuzzleOutput PuzzleOutput { get; }

    public bool CompletionCondition(Func<bool> condition, bool reset) ;
}