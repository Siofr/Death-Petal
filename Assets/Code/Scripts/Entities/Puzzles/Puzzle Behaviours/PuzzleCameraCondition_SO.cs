using System;
using UnityEngine;

public abstract class PuzzleCameraCondition_SO : ScriptableObject
{
    public Func<bool> exitCondition;
    public float enteredTime;
}