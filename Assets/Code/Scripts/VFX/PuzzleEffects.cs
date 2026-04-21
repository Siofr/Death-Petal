using System.Collections.Generic;
using UnityEngine;

public class PuzzleEffects : MonoBehaviour
{
    private EventBindings<WrongShotPuzzleEvent> _wrongShotPuzzleEventListener;
    
    public List<GameObject> wrongShotEffectObjects;

    private void OnEnable()
    {
        _wrongShotPuzzleEventListener = new EventBindings<WrongShotPuzzleEvent>(OnWrongShot);
        EventBus<WrongShotPuzzleEvent>.Register(_wrongShotPuzzleEventListener);
    }

    private void OnDisable()
    {
        EventBus<WrongShotPuzzleEvent>.Unregister(_wrongShotPuzzleEventListener);
    }

    private void OnWrongShot(WrongShotPuzzleEvent ctx)
    {
        //print("wrong shot particles spawning");
        if (ctx.weight != GetComponentInParent<Weight>())
            return;

        foreach (var effect in wrongShotEffectObjects)
        {
            effect.SetActive(true);
        }
    }
}
