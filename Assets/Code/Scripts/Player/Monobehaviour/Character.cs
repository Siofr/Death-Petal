using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private Dictionary<string, StateMachine> _layerMachines = new Dictionary<string, StateMachine>();
}
