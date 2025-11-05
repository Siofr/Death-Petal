using UnityEngine;

public class Weakness : MonoBehaviour, IWeakness
{
    [SerializeField] WeakTypes _weaknessType;
    
    
    public WeakTypes WeakType { get; }
}
