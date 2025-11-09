using System;
using UnityEngine;

[Flags]
public enum WeakTypes {
    NONE,
    RED,
    BLUE,
    GREEN,
    PLAYER = Int32.MaxValue
}

[RequireComponent(typeof(SphereCollider))]
public class Weakness : MonoBehaviour
{
    [SerializeField] WeakTypes _weaknessType;
    [SerializeField] private SphereCollider _collider;
    
    private IEntity _parentEntity;
    
    public WeakTypes WeakType => _weaknessType;
    public IEntity ParentEntity => _parentEntity;
    
    private void Awake()
    {
        _collider ??= GetComponent<SphereCollider>();
        _parentEntity ??= GetComponentInParent<IEntity>();
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = Color.clear;
        
        switch (_weaknessType)
        {
            case WeakTypes.RED:
                gizmoColor = Color.red;
                break;
            case WeakTypes.BLUE:
                gizmoColor = Color.blue;
                break;
            case WeakTypes.GREEN:
                gizmoColor = Color.green;
                break;
        }   
        
        Gizmos.color = gizmoColor;

        if(_collider !=null)
            Gizmos.DrawWireSphere(transform.position, _collider.radius);
    }
}
