

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using FMODUnity;

public class Door : PuzzleOutputBase
{
    [Header("Door Fields")]
    [Header("In Seconds")]
    public float openSpeed;

    //Non-Serializable fields
    private Queue<IEnumerator> _routineQueue = new Queue<IEnumerator>();

    private bool CheckBlendState(bool isOpened)
    {
        float value = isOpened ? 1f : 0f;
        return animator.GetFloat(Animator.StringToHash("Blend")) == value;
    }
    
    private IEnumerator OpenDoorRoutine(bool isOpened, float openSpeed, bool isEditor = false)
    {
        Debug.Log("Starting OpenDoor Routine");
        
        RuntimeManager.PlayOneShot(onCompletionEventPath, transform.position);

        float updateStep = isEditor ? 1 / 60f : Time.deltaTime;

        float value = isOpened ? 0f : 1f;

        if (_camera != null) yield return new WaitForSeconds(_cameraPanTime);
        
        StartPanningCamera(() => CheckBlendState(isOpened));
        
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
#if UNITY_EDITOR
            if (!isEditor)
            {
                EditorCoroutineUtility.StartCoroutine(OpenDoorRoutine(isOpened, openSpeed, true), this);
                return;
            }
            #endif
            
            StartCoroutine(OpenDoorRoutine(isOpened, openSpeed, false));
        }
    }

    public void ClearAnimationQueue()
    {
        _routineQueue.Clear();
    }
}