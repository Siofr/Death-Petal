using System;
using System.Collections.Generic;
using System.Collections;
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

public struct PlayerHealedEvent : IEvent { }

public struct PlayerLowHealthEvent : IEvent { }

public struct UpdateHealthEvent : IEvent { }

public struct PlayerDeathEvent : IEvent { }

public class TestPlayer : EntityBase, IEntity
{
    [Header("Player Fields")] 
    [SerializeField] private float _damageCooldown;
    
    //private EventBindings<PetalPickpEvent> _petalPickupEventListener;

    private int _maxHealth = 3;
    private int _currentHealth;
    private int _currentPetalCharge;
    private int _goalPetalCharge = 3;

    private EventBindings<PlayerLowHealthEvent> _onPlayerLowHealth;
    private EventBindings<PlayerHealedEvent> _onPlayerHealedEvent;

    private bool _isCritical;

    public void Awake()
    {
        base.Awake();

        _onPlayerLowHealth = new EventBindings<PlayerLowHealthEvent>(OnPlayerCriticalHealth);
        _onPlayerHealedEvent = new EventBindings<PlayerHealedEvent>(OnPlayerRecovered);
        // _currentHealth = _maxHealth;
        // _petalPickupEventListener = new EventBindings<PetalPickpEvent>(OnPetalCollected);
    }

    private void OnEnable()
    {
        //EventBus<PetalPickpEvent>.Register(_petalPickupEventListener);
        EventBus<PlayerLowHealthEvent>.Register(_onPlayerLowHealth);
        EventBus<PlayerHealedEvent>.Register(_onPlayerHealedEvent);
    }

    private void OnDisable()
    {
        //EventBus<PetalPickpEvent>.Unregister(_petalPickupEventListener);
        EventBus<PlayerLowHealthEvent>.Unregister(_onPlayerLowHealth);
        EventBus<PlayerHealedEvent>.Unregister(_onPlayerHealedEvent);
    }

    /*private void OnPetalCollected()
    {
        _currentPetalCharge++;

        if (_currentPetalCharge >= _goalPetalCharge)
        {
            EventBus<ChangeScoreEvent>.Raise(new ChangeScoreEvent("Full Petal", 100));
            _currentPetalCharge = 0;

            if (Weaknesses.Count < _maxHealth)
            {
                Weaknesses.Add(new Weakness());
                //DEPRECATED
                //OnPlayerHealthChange();
            }

            return;
        }

        EventBus<ChangeScoreEvent>.Raise(new ChangeScoreEvent("Petal", 10));
    }*/

    private float _lastDamageTime;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (damageType == WeakTypes.PLAYER)
        {
            var tempDamageTime = Time.time;

            if (_lastDamageTime != 0 && tempDamageTime - _lastDamageTime < _damageCooldown) return;

            _lastDamageTime = tempDamageTime;
            
            //print("player damaged");
            EventBus<PlayerDamageEvent>.Raise(new PlayerDamageEvent(this));
            EventBus<WipeComboEvent>.Raise(new WipeComboEvent());
            Weaknesses.Remove(weakness);

            Destroy(weakness.transform.parent.gameObject);   
            EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent(Weaknesses.Count));
        }
        
        if(Weaknesses.Count == 1) EventBus<PlayerLowHealthEvent>.Raise(new PlayerLowHealthEvent());
        
        if (Weaknesses.Count < 1)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            //print("Player diad");
            EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());
        }

        //DEPRECATED
        //OnPlayerHealthChange();
    }


    // private void OnPlayerHealthChange()
    // {
    //     _currentHealth = Weaknesses.Count;
    //
    //     if (_currentHealth == 1)
    //     {
    //         StartCoroutine(LowHealthEffect());
    //     }
    // }

    private void OnPlayerCriticalHealth(PlayerLowHealthEvent ctx)
    {
        _isCritical = true;
        StartCoroutine(LowHealthEffect());
    }

    private void OnPlayerRecovered(PlayerHealedEvent ctx)
    {
        _isCritical = false;
    }

    IEnumerator LowHealthEffect()
    {
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.75f, 0.75f, 0.15f));

        yield return new WaitForSeconds(1.5f);

        if (!_isCritical) yield return null;
        else StartCoroutine(LowHealthEffect());
    }

    public override void HandleLoadData(ref LevelSaveData refData)
    {
        base.HandleLoadData(ref refData);

        var diff = Weaknesses.Count - SaveInfo.health.Count;

        if (diff > 0)
        {
            for (int i = Weaknesses.Count - 1; i > SaveInfo.health.Count - 1; i--)
            {
                var tempWeakness =  Weaknesses[i];
                Weaknesses.Remove(tempWeakness);
                Destroy(tempWeakness.transform.parent.gameObject);
            }
        }
        
        var healthCount = Weaknesses.Count;
        
        if(healthCount < 3) EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent());
        
        if (healthCount < 2) EventBus<PlayerLowHealthEvent>.Raise(new PlayerLowHealthEvent());
        else EventBus<PlayerHealedEvent>.Raise(new PlayerHealedEvent());
    }
}