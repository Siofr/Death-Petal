using System;
using UnityEditor;
using UnityEngine;

public class WeaknessFactory : Singleton<WeaknessFactory>
{
    [SerializeField] GameObject _weaknessPrefab;
    [SerializeField] EntityBase _player;
    
    //Events
    private EventBindings<PetalPickpEvent> _petalPickupListener;
    
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
    
    private void Awake()
    {
        GameObject.FindWithTag("Player").TryGetComponent(out _player);
    }

    private void OnEnable()
    {
        _petalPickupListener = new EventBindings<PetalPickpEvent>(CreatePlayerWeakness);
        EventBus<PetalPickpEvent>.Register(_petalPickupListener);
    }

    private void OnDisable()
    {
        EventBus<PetalPickpEvent>.Unregister(_petalPickupListener);
    }
}