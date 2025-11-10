public interface IPuzzleOutput
{
    public void OnPuzzleSolved(PuzzleSolvedEvent context);
    public void OnPuzzleReset(PuzzleResetEvent context);
}