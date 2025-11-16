

using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public class Door : PuzzleOutputBase
{
    [Header("Door Fields")]
    [Header("In Seconds")]
    public float openSpeed;
    
    //Non-Serializable fields
    private Queue<IEnumerator> _routineQueue = new Queue<IEnumerator>();
    
    private IEnumerator OpenDoorRoutine(bool isOpened, float openSpeed, bool isEditor = false)
    {
        Debug.Log("Starting OpenDoor Routine");
        
        float updateStep = isEditor ? 1 / 60f : Time.deltaTime;

        float value = isOpened ? 0f : 1f;
        
        for (float i = 0; i < 1; i += updateStep/openSpeed)
        {
            value = isOpened ? i : 1 - i;
            
            animator.SetFloat(Animator.StringToHash("Blend"), value);
            
            updateStep = isEditor ? 1 / 60f : Time.deltaTime;
            yield return null;
        }

        value = isOpened ? 1 : 0; 
        animator.SetFloat(Animator.StringToHash("Blend"), value);
        
        _routineQueue.Dequeue();
        
        Debug.Log("Finished Routine");
        
        animator.SetBool(Animator.StringToHash("IsSolved"), isOpened);
        
        if (_routineQueue.Count > 0)
        {
            Debug.Log("Starting next Routine in Queue");
            
            yield return _routineQueue.Peek();
        }
    }

    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        if (context.puzzleOutput != this) return;

        IsSolved = true;
        
        OpenDoor(openSpeed, true, false);
    }

    public override void OnPuzzleReset(PuzzleResetEvent context)
    {
        if (context.puzzleOutput != this) return;
        
        IsSolved = false;
        
        OpenDoor(openSpeed, false, false);
    }

    public void OpenDoor(float openSpeed, bool isOpened, bool isEditor)
    {
        Debug.Log(_routineQueue.Count);
        
        _routineQueue.Enqueue(OpenDoorRoutine(isOpened, openSpeed, isEditor));
        
        if (_routineQueue.Count < 2)
        {
            if (!isEditor)
            {
                EditorCoroutineUtility.StartCoroutine(OpenDoorRoutine(isOpened, openSpeed, true), this);
                return;
            }
            
            StartCoroutine(OpenDoorRoutine(isOpened, openSpeed, false));
        }
    }

    public void ClearAnimationQueue()
    {
        _routineQueue.Clear();
    }
}