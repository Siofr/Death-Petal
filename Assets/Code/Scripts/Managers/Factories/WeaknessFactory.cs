using System;
using UnityEditor;
using UnityEngine;

public struct CreatePlayerWeaknessEvent: IEvent
{
    public Transform target;
    public WeakTypes weakType;

    public CreatePlayerWeaknessEvent(Transform target, WeakTypes weakType)
    {
        this.target = target;
        this.weakType = weakType;
    }
}

public struct CreateWeaknessEvent : IEvent
{
    public EntityBase entity;
    public WeakTypes weakType;
    public Vector3 iconPosition;

    public CreateWeaknessEvent(EntityBase entity, WeakTypes weakType, Vector3 iconPosition)
    {
        this.entity = entity;
        this.weakType = weakType;
        this.iconPosition = iconPosition;
    }
}

public class WeaknessFactory : Singleton<WeaknessFactory>
{
    [SerializeField] GameObject _weaknessPrefab;
    [SerializeField] EntityBase _player;
    
    //Events
    private EventBindings<PetalPickpEvent> _petalPickupListener;
    private EventBindings<CreatePlayerWeaknessEvent> _createPlayerWeaknessListener;
    private EventBindings<CreateWeaknessEvent> _createWeaknessListener;
    
    //Properties
    public GameObject ProductPrefab => _weaknessPrefab;

    public void CreateWeakness(EntityBase entity, WeakTypes weakType, Vector3 iconPosition)
    {
        var weaknesssContainer = entity.transform.Find("Weaknesses");

        if (weaknesssContainer == null) return;
        
        var instance = Instantiate(_weaknessPrefab, weaknesssContainer);
        var weakness = instance.GetComponentInChildren<Weakness>();
        
        weakness.Initialise(weakType, iconPosition);
        entity.InitialiseWeaknesses();
    }

    public void CreatePlayerWeakness()
    {
        if (_player == null) return;
        CreateWeakness(_player, WeakTypes.PLAYER, _player.transform.position);
    }

    public void OnWeaknessRequest(CreateWeaknessEvent ctx)
    {
        CreateWeakness(ctx.entity, ctx.weakType, ctx.iconPosition);
    }
    
    private void Awake()
    {
        GameObject.FindWithTag("Player").TryGetComponent(out _player);
    }

    private void OnEnable()
    {
        _petalPickupListener = new EventBindings<PetalPickpEvent>(CreatePlayerWeakness);
        _createPlayerWeaknessListener = new EventBindings<CreatePlayerWeaknessEvent>(CreatePlayerWeakness);
        _createWeaknessListener = new EventBindings<CreateWeaknessEvent>(OnWeaknessRequest);
        
        EventBus<CreatePlayerWeaknessEvent>.Register(_createPlayerWeaknessListener);
        EventBus<PetalPickpEvent>.Register(_petalPickupListener);
        EventBus<CreatePlayerWeaknessEvent>.Register(_createPlayerWeaknessListener);
    }

    private void OnDisable()
    {
        EventBus<PetalPickpEvent>.Unregister(_petalPickupListener);
        EventBus<CreatePlayerWeaknessEvent>.Unregister(_createPlayerWeaknessListener);
        EventBus<CreateWeaknessEvent>.Unregister(_createWeaknessListener);
    }
}