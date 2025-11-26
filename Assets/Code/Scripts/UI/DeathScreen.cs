using System;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    //public GameObject DeathUiObject;
    public Vector3 particleOffset;
    public float rotationSpeed;
    
    private Camera _deathCamera;
    private GameObject _deathParticles;
    private GameObject _player;
    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        
        _deathCamera = GetComponentInChildren<Camera>();
        _deathParticles = GameObject.Find("PlayerDeathParticles");
    }

    private void OnEnable()
    {
        SetupCamera();
        SetupParticles();
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
