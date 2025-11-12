public interface IPuzzleOutput
{
    public bool IsSolved { get; set; }
    public void OnPuzzleSolved(PuzzleSolvedEvent context);
    public void OnPuzzleReset(PuzzleResetEvent context);
}