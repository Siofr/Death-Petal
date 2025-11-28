public interface IPuzzleOutput
{
    public bool IsSolved { get; set; }
    public void SetSolved(bool solved);
    public void OnPuzzleSolved(PuzzleSolvedEvent context);
    public void OnPuzzleReset(PuzzleResetEvent context);
}