using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDamageEvent: IEvent
{
    public TestPlayer testPlayer;
    
    public PlayerDamageEvent(TestPlayer testPlayerRefrence) => testPlayer =  testPlayerRefrence;
}

public struct PlayerDamagedEvent : IEvent
{
    
}

public struct PlayerDeathEvent : IEvent { }

{
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (damageType == WeakTypes.PLAYER)
        {
            print("player damaged");
            EventBus<PlayerDamageEvent>.Raise(new PlayerDamageEvent(this));
            Weaknesses.Remove(weakness);

            Destroy(weakness.transform.parent.gameObject);   
            EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent());

        }
        
        if (Weaknesses.Count < 1)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            print("Player diad");
            EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());
        }
    }
}