using System;
using UnityEngine;

[Flags]
public enum WeakTypes {
    NONE = 0,
    PLAYER = 2,
    RED = 4,
    BLUE = 8,
    GREEN = 16
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
        _collider = GetComponent<SphereCollider>();
        _parentEntity = GetComponentInParent<IEntity>();
    }

    private void Start()
    {
        if(_parentEntity != null && !_parentEntity.Weaknesses.Contains(this)) _parentEntity.Weaknesses.Add(this);
    }

    /*private void OnDrawGizmos()
    {
        var gizmoColor = _weaknessType switch
        {
            WeakTypes.RED => Color.red,
            WeakTypes.BLUE => Color.blue,
            WeakTypes.GREEN => Color.green,
            WeakTypes.RED | WeakTypes.BLUE => Color.purple,
            WeakTypes.RED | WeakTypes.GREEN => Color.yellow,
            WeakTypes.BLUE | WeakTypes.GREEN => Color.cyan,
            WeakTypes.RED | WeakTypes.BLUE | WeakTypes.GREEN => Color.white,
            _ => Color.clear
        };

        Gizmos.color = gizmoColor;

        if(_collider !=null)
            Gizmos.DrawSphere(transform.position, _collider.radius);
    }*/
}
