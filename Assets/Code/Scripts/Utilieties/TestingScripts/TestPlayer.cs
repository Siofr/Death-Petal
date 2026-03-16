using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDamageEvent: IEvent
{
    public TestPlayer testPlayer;
    
    public PlayerDamageEvent(TestPlayer testPlayerRefrence) => testPlayer =  testPlayerRefrence;
}

public struct PlayerDamagedEvent : IEvent
{
    public int health;

    public PlayerDamagedEvent(int health)
    {
        this.health = health;
    }
}

public struct PlayerDeathEvent : IEvent { }

public class TestPlayer : EntityBase, IEntity
{
    private EventBindings<PetalPickpEvent> _petalPickupEventListener;

    private int _maxHealth = 3;
    private int _currentPetalCharge;
    private int _goalPetalCharge = 3;

    public void Awake()
    {
        base.Awake();

        _petalPickupEventListener = new EventBindings<PetalPickpEvent>(OnPetalCollected);
    }

    private void OnEnable()
    {
        EventBus<PetalPickpEvent>.Register(_petalPickupEventListener);
    }

    private void OnDisable()
    {
        EventBus<PetalPickpEvent>.Unregister(_petalPickupEventListener);
    }

    private void OnPetalCollected()
    {
        _currentPetalCharge++;

        if (_currentPetalCharge >= _goalPetalCharge && Weaknesses.Count < _maxHealth)
        {
            Weaknesses.Add(new Weakness());
            _currentPetalCharge = 0;
        }
    }

    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (damageType == WeakTypes.PLAYER)
        {
            print("player damaged");
            EventBus<PlayerDamageEvent>.Raise(new PlayerDamageEvent(this));
            Weaknesses.Remove(weakness);

            Destroy(weakness.transform.parent.gameObject);   
            EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent(Weaknesses.Count));

        }
        
        if (Weaknesses.Count < 1)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            print("Player diad");
            EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());
        }
    }
}