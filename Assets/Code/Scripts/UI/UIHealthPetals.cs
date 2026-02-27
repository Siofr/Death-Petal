using System;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthPetals : MonoBehaviour
{
    public GameObject healthContainer;
    public int maxHealth;
    public int visualHealth = 27;

    private List<Animator> _healthPedalsAnimators;
    private IEntity _healthData;
    private int _petalHealthValue;
    
    public GameObject player;

    public void Awake()
    {
        _healthPedalsAnimators = new List<Animator>();
        _healthData = player.GetComponent<IEntity>();
        
        _petalHealthValue = visualHealth / maxHealth;
        
        for (int i = 0; i < visualHealth; i++)
        {
            _healthPedalsAnimators.Add(healthContainer.transform.GetChild(i).GetComponent<Animator>());
        }
    }

    public void Update()
    {
        //TODO: Trigger this on an a player damage event!
        UpdateHealthPedals();
    }

    void UpdateHealthPedals()
    {
        for (int i = 0; i < visualHealth; i++)
        {
            bool IsHpOn = (i - _petalHealthValue < _healthData.Weaknesses.Count * _petalHealthValue);
            bool IsLast = (IsHpOn && _healthData.Weaknesses.Count == 1);
            
            _healthPedalsAnimators[i].SetBool("HP",  IsHpOn);
            _healthPedalsAnimators[i].SetBool("Last",  IsLast);
            
            //print("HP: " + i.ToString() +  " " + IsHpOn.ToString());
        }
        
        
    }
}
