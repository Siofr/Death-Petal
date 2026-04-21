using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Simple Timed Camera", menuName = "Camera Pan Conditions/Timed Camera", order = 0)]
public class PuzzleCameraSimpleTimed : PuzzleCameraCondition_SO
{
    public float timeToWait = 3;
    
    private void OnEnable()
    {
        exitCondition = new Func<bool>(ExitCondition);
    }

    private bool ExitCondition()
    {
        return (Time.time - enteredTime >= timeToWait);
    }
}
