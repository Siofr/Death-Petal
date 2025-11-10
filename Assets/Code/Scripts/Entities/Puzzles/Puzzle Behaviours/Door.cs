using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : PuzzleOutputBase
{
    [Header("Door Fields")]
    [SerializeField] private Transform _pivotPoint;
    [SerializeField] private float _openAngle;
    [Header("In Seconds")]
    [SerializeField] private float _openSpeed;
    
    //Non-Serializable Fields
    private Queue<Action> _doorOpenCalls =  new Queue<Action>();

    private void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        _doorOpenCalls.Clear();
    }
    
    private IEnumerator OpenDoorRoutine(float angle, float speed, bool isOpening)
    {
        var goalAngle = isOpening ? _openAngle : -_openAngle;
        var tempAngle = 0;

        var condition = tempAngle < angle;
        if (!isOpening) condition = tempAngle > angle;

        while (condition)
        {
            var angleStep = Time.deltaTime / speed;

            transform.RotateAround(_pivotPoint.position, Vector3.up, angleStep);

            yield return null;
        }

        _doorOpenCalls.Dequeue();
        
        if(_doorOpenCalls.Count > 0)
            _doorOpenCalls.Peek().Invoke();
    }

    public void OpenDoor(bool isOpening)
    {
        StartCoroutine(OpenDoorRoutine(_openAngle, _openSpeed, isOpening));
    }
    
    private void AddOrCallDoorOpen(bool isOpening)
    {
        Action openDoorAction = () => { };
        openDoorAction += () => { OpenDoor(isOpening); };
        
        if (_doorOpenCalls.Count < 1)
        {
            _doorOpenCalls.Enqueue(openDoorAction);
            openDoorAction.Invoke();
        }
        else
        {
            _doorOpenCalls.Enqueue(openDoorAction);
        }        
    }
    
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        AddOrCallDoorOpen(true);
    }

    public override void OnPuzzleReset(PuzzleResetEvent context)
    {
        AddOrCallDoorOpen(false);
    }
}