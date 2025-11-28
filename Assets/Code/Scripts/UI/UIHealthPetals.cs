using System;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthPetals : MonoBehaviour
{
    public GameObject healthContainer;
    public int maxHealth;

    private List<Animator> _healthPedalsAnimators;
    private IEntity _healthData;
    
    public GameObject player;

    public void Awake()
    {
        _healthPedalsAnimators = new List<Animator>();
        _healthData = player.GetComponent<IEntity>();
        
        for (int i = 0; i < maxHealth; i++)
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
        for (int i = 0; i < maxHealth; i++)
        {
            bool IsHpOn = (i < _healthData.Weaknesses.Count);
            bool IsLast = (IsHpOn && _healthData.Weaknesses.Count == 1);
            
            _healthPedalsAnimators[i].SetBool("HP",  IsHpOn);
            _healthPedalsAnimators[i].SetBool("Last",  IsLast);
            
            //print("HP: " + i.ToString() +  " " + IsHpOn.ToString());
        }
    }
}
