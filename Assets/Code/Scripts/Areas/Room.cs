using System;
using JetBrains.Annotations;
using UnityEngine;

public struct RoomPlayerEnterEvent: IEvent
{
    public Transform playerTransform;
    
    public RoomPlayerEnterEvent(Transform playerTransform, Room room)
    {
        this.playerTransform = playerTransform;
    }
}

public struct RoomPlayerExitEvent : IEvent
{
    public Room room;
    
    public RoomPlayerExitEvent(Room room)
    {
        this.room = room;
    }
}

[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour
{
    [SerializeField] private Transform _roomCameraTransform;
    private BoxCollider _collider;    
    
    public Bounds Bounds => _collider.bounds;
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _roomCameraTransform.gameObject.SetActive(true);
            EventBus<RoomPlayerEnterEvent>.Raise(new RoomPlayerEnterEvent(other.transform, this));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _roomCameraTransform.gameObject.SetActive(false);
            EventBus<RoomPlayerExitEvent>.Raise(new RoomPlayerExitEvent(this));
        }
    }
} 