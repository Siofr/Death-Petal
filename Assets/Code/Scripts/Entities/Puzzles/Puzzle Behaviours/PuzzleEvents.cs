using UnityEngine;

public struct PuzzleSolvedEvent : IEvent
{
    public IPuzzleOutput puzzleOutput;
    
    public PuzzleSolvedEvent(IPuzzleOutput puzzleOutput) => this.puzzleOutput = puzzleOutput;
}

public struct PuzzleResetEvent : IEvent
{
    public IPuzzleOutput puzzleOutput;
    
    public PuzzleResetEvent(IPuzzleOutput puzzleOutput) => this.puzzleOutput = puzzleOutput;
}