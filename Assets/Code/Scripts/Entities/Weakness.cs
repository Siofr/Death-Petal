using System;
using System.Collections;
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

    //[SerializeField] private Sprite[] _weaknessTextures;
    
    //Non-Serializable
    private MeshRenderer _renderer;
    private GameObject _player;
    
    private IEntity _parentEntity;
    
    public WeakTypes WeakType => _weaknessType;
    public IEntity ParentEntity => _parentEntity;
    
    public Transform WeaknessIconTransform => _weaknessIconTransform;
    
    public void RemoveWeakType(WeakTypes weakType)
    {
        _weaknessType &= ~weakType;
    }

    public void SetWeakType(WeakTypes weakType)
    {
        _weaknessType = weakType;
        Initialise();
    }

    public void ToggleHitbox(bool toggle)
    {
        _collider.enabled = toggle;
    }

    public void ToggleIcon(bool toggle)
    {
        if (_renderer == null) return;
        
        var value = toggle ? .12f : 1f;
        _renderer.material.SetFloat("_Opacity", value);
    }

    public void Toggle(bool toggle)
    {
        ToggleHitbox(toggle);
        ToggleIcon(toggle);
    }
    
    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _parentEntity = GetComponentInParent<IEntity>();
        _renderer = _weaknessIconTransform.GetComponent<MeshRenderer>();   
        
        Initialise();

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Initialise(WeakTypes weakType, Vector3 iconPosition)
    {
        _weaknessType = weakType;
        _weaknessIconTransform.position = iconPosition;
        SetWeaknessColor();
    }

    public void Initialise()
    {
        Initialise(_weaknessType, _weaknessIconTransform.position);
    }

    public void SetWeaknessColor()
    {
        if (!Application.isPlaying) return;
        
        _renderer.material.color = _weaknessType switch
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

        int tex = _weaknessType switch
        {
            WeakTypes.RED => 1,
            WeakTypes.BLUE => 0,
            WeakTypes.GREEN => 3,
            _ => _renderer.material.GetInt("_TextureIndex")
        };
        
        _renderer.material.SetInt("_TextureIndex", tex);
        
        if(WeakType == WeakTypes.PLAYER || WeakType == WeakTypes.NONE) _renderer.material.SetFloat("_Opacity", 1f);
        else _renderer.material.SetFloat("_Opacity", .12f);
    }

    public void StartDelayDestroy()
    {
        Toggle(false);
        Destroy(transform.parent.gameObject);
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
