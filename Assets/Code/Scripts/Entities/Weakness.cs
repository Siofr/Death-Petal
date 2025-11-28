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
    [SerializeField] private Transform _weaknessIconTransform;
    
    //Non-Serializable
    private MeshRenderer _renderer;
    private GameObject _player;
    private Material _weaknessMaterial;
    
    private IEntity _parentEntity;
    
    public WeakTypes WeakType => _weaknessType;
    public IEntity ParentEntity => _parentEntity;
    
    public Transform WeaknessIconTransform => _weaknessIconTransform;
    
    
    public void RemoveWeakType(WeakTypes weakType)
    {
        _weaknessType &= ~weakType;
        SetWeaknessColor();
    }

    public void RemoveWeakness()
    {
        if(!Application.isPlaying) DestroyImmediate(transform.parent.gameObject);
        else Destroy(transform.parent.gameObject);
    }
    
    public void SetWeaknessType(WeakTypes weakType)
    {
        _weaknessType = weakType;
    }

    public void Initialize(WeakTypes weakType)
    {
        _weaknessType = weakType;
        _collider = GetComponent<SphereCollider>();
        _parentEntity = GetComponentInParent<IEntity>();
        _renderer = _weaknessIconTransform.GetComponent<MeshRenderer>();
        
        if(Application.isPlaying)
            SetWeaknessColor();

        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void Awake()
    {
        Initialize(_weaknessType);
    }

    private void SetWeaknessColor()
    {
        if (_weaknessMaterial == null)
            _weaknessMaterial = _renderer.material;
        
        _weaknessMaterial.color = _weaknessType switch
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
        
        _renderer.material = _weaknessMaterial;
    }
    
    private void Update()
    {
        if (_player != null)
        {
            var temp = transform.position;
            temp.y = _player.transform.position.y;
            transform.position = temp;
        }
    }

    private void OnDestroy()
    {
        if(ParentEntity.Weaknesses.Contains(this)) ParentEntity.Weaknesses.Remove(this);
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
