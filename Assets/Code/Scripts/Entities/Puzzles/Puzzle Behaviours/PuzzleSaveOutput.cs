using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleSaveOutput : PuzzleOutputBase
{
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if (IsSolved) return;

        if (context.puzzleOutput != this) return;
        
        EventBus<LevelSaveEvent>.Raise(new LevelSaveEvent());
        
        if(!SceneManager.GetSceneByName("SavingUI").isLoaded) SceneManager.LoadSceneAsync("SavingIcon", LoadSceneMode.Additive);
        
        base.OnPuzzleSolved(context);
    }

    protected override void Start()
    {
        if (IsSolved) return;
    }
}
