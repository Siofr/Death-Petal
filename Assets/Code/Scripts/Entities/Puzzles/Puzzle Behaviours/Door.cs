

public class Door : PuzzleOutputBase
{
    public void OpenDoor()
    {
        OnPuzzleSolved(new PuzzleSolvedEvent());
    }

    public void CloseDoor()
    {
        OnPuzzleSolved(new PuzzleSolvedEvent());
    }
}