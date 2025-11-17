using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleInput: IEntity
{
    public List<IPuzzleOutput> PuzzleOutputs { get; }

    public bool CompletionCondition(bool condition, IPuzzleOutput targetOutput) ;
}