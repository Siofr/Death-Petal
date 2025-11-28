using System;
using UnityEngine;

public struct PetalPickpEvent : IEvent { }

[RequireComponent(typeof(SphereCollider))]
public class Petal : MonoBehaviour
{
    private Collider _collider;
    private Collider _playerCollider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        GameObject.FindWithTag("Player").TryGetComponent(out _playerCollider);
    }

    public bool IntersectionCheck()
    {
        return _collider.bounds.Intersects(_playerCollider.bounds);
    }
    
    private void Update()
    {
        if (_playerCollider == null) return;
        
        if(IntersectionCheck()) Destroy(gameObject);
    }
    
    public void OnDestroy()
    {
        EventBus<PetalPickpEvent>.Raise(new PetalPickpEvent());
    }
}