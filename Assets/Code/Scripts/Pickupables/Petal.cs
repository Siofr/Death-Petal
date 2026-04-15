using System;
using UnityEngine;

public struct PetalPickpEvent : IEvent { }

[RequireComponent(typeof(SphereCollider))]
public class Petal : MonoBehaviour
{
    [Header("Petal Fields")]
    [Header("In Units/Second")]
    [SerializeField] private float _petalHoverSpeed;

    [SerializeField] private float _petalHoverDistance;
    
    //Non-Serializable Fields
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

        Hover();
        if(IntersectionCheck()) Destroy(gameObject);
    }

    public void Hover()
    {
        if (Vector3.Distance(transform.position, _playerCollider.transform.position) > _petalHoverDistance) return;
        transform.position += (_playerCollider.transform.position-transform.position).normalized *  _petalHoverSpeed * Time.deltaTime;
    }
    
    public void OnDestroy()
    {
        EventBus<PetalPickpEvent>.Raise(new PetalPickpEvent());
    }
}