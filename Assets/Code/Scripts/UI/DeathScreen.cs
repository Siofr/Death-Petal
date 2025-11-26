using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    //public GameObject DeathUiObject;
    public Vector3 particleOffset;
    public float rotationSpeed;

    public GameObject DeathPlayer;
    public Button startButton;
    
    private Camera _deathCamera;
    private GameObject _deathParticles;
    private GameObject _player;
    private EventSystem _eventSystem;
    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        
        _deathCamera = GetComponentInChildren<Camera>();
        _deathParticles = GameObject.Find("PlayerDeathParticles");
        _eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        SetupDeadPlayer();
        SetupCamera();
        SetupParticles();
    }

    void SetupDeadPlayer()
    {
        Instantiate(DeathPlayer, _player.transform.position, _player.transform.rotation);
    }

    void SetupCamera()
    {
        Camera _mainCamera = Camera.main;
        _deathCamera.fieldOfView = _mainCamera.fieldOfView;
        
        _deathCamera.transform.rotation = _mainCamera.transform.rotation;
        _deathCamera.transform.position = _mainCamera.transform.position;
    }

    void SetupParticles()
    {
        _deathParticles.transform.position = _player.transform.position + particleOffset;
    }

    void SetupButtons()
    {
        _eventSystem.SetSelectedGameObject(startButton.gameObject);
    }

    void Update()
    {
        SpinCamera();
    }
    
    void SpinCamera()
    {
        _deathCamera.transform.RotateAround(_player.transform.position, 
            Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
