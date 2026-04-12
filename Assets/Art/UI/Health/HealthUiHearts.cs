using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUiHearts : MonoBehaviour
{
    private IEntity _healthData;
    public int maxHp = 3;
    
    public GameObject player;

    public GameObject healthImage;

    public List<GameObject> _hearts;
    private EventBindings<PlayerDamagedEvent> _playerDamageEventListener;

    void Awake()
    {
        _playerDamageEventListener = new EventBindings<PlayerDamagedEvent>(UpdateHealthPedals);
    }


    void OnEnable()
    {
        EventBus<PlayerDamagedEvent>.Register(_playerDamageEventListener);


        _healthData = player.GetComponent<IEntity>();

        //maxHp = _healthData.Weaknesses.Count;

        //InnitHearts();
        InnitVisuals();
    }

    void OnDisable()
    {
        EventBus<PlayerDamagedEvent>.Unregister(_playerDamageEventListener);
    }

    void InnitHearts()
    {
        bool first = true;
        foreach(var child in GetComponentsInChildren<Transform>())
        {
            if(!first)
            {
                Destroy(child.gameObject);
            }
            else
            {
                first = false;
            }
        } 

        for (int i = 0; i < maxHp; i++)
        {
            var currentHeart = Instantiate(healthImage);
            _hearts.Add(currentHeart);

            currentHeart.transform.parent = transform;

            //print("[HEARTS] : innit heart " + i);
        }
    }

    void InnitVisuals()
    {
        Material refMaterial = _hearts[0].GetComponent<RawImage>().material;

        for (int i = 0; i < _hearts.Count; i++)
        {
            Material tempMat = new Material(refMaterial);

            tempMat.SetFloat("_Index", 9 + (i % 3));

            _hearts[i].GetComponent<RawImage>().material = tempMat;
        }
    }

    void Update()
    {
        //TODO: Trigger this on an a player damage event!
        //UpdateHealthPedals();
    }

    void UpdateHealthPedals()
    {
        for (int i = 0; i < maxHp; i++)
        {
            //print(i);
            _hearts[i].GetComponent<RawImage>().enabled = i < (_healthData.Weaknesses.Count);
        }

        //print("[HP] " + _healthData.Weaknesses.Count);
    }
}
