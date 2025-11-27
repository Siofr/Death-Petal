using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    //public GameObject DeathUiObject;
    public Vector3 particleOffset;
    public float rotationSpeed;

    [FormerlySerializedAs("DeathPlayer")] public GameObject deathPlayer;
    public GameObject particleRoot;
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
        SetupButtons();
    }

    void SetupDeadPlayer()
    {
        //var deadPlayer = Instantiate(DeathPlayer, _player.transform.position, _player.transform.rotation);
        //deadPlayer.transform.SetParent(gameObject.transform);

        deathPlayer.transform.position = _player.transform.position;
        deathPlayer.transform.rotation = _player.transform.rotation;

        //Debug.LogWarning(deadPlayer.name);
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
        _deathParticles.transform.position = particleRoot.transform.position + particleOffset;
    }

    void SetupButtons()
    {
        _eventSystem.SetSelectedGameObject(startButton.gameObject);
    }

    void Update()
    {
        SpinCamera();
        SetupParticles();
    }
    
    void SpinCamera()
    {
        _deathCamera.transform.RotateAround(_player.transform.position, 
            Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
