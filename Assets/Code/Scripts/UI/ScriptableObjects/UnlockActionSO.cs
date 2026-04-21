using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UnlockAction", menuName = "ScriptableObjects/UnlockAction", order = 1)]
public class UnlockActionSO : ScriptableObject
{
    public List<InputAction> unlockedActions = new List<InputAction>();

    public void ActivateInput()
    {
        foreach(InputAction unlockedAction in unlockedActions)
        {
            unlockedAction.Enable();
        }
    }
}
