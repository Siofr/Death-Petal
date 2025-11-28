using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDamageEvent: IEvent{ }

public struct PlayerDamagedEvent : IEvent
{
    
}

public class TestPlayer : EntityBase
{
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (damageType == WeakTypes.PLAYER)
        {
            EventBus<PlayerDamageEvent>.Raise(new PlayerDamageEvent());
            Weaknesses.Remove(weakness);

            Destroy(weakness.transform.parent.gameObject);   
            EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent());

        }
        
        if (Weaknesses.Count < 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}