using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Centralised EventBus
/// 
/// Manages EventBindings in a HashSet for quick access
/// Sends a message to any listener subscribed to the Bus with event type T
/// </summary>
/// <typeparam name="T">Specifies event type for the Bus</typeparam>
public static class EventBus<T> where T : IEvent
{
    //Storage of listeners for event types of T
    private static HashSet<EventBindings<T>> _eventBindings = new HashSet<EventBindings<T>>();
    //Storage of Disabled Events
    private static HashSet<Type> _disabledEvents = new HashSet<Type>();
    
    //Add listener to Bus of type T
    public static void Register(EventBindings<T> eventBindings) =>  _eventBindings.Add(eventBindings);
    //Remove listener to Bus of type T
    public static void Unregister(EventBindings<T> eventBindings) => _eventBindings.Remove(eventBindings);
    
    //Disable Event
    public static void DisableEvent() => _disabledEvents.Add(typeof(T));
    
    //Enable Event
    public static void EnableEvent() => _disabledEvents.Remove(typeof(T));
    
    //Send Message to listeners of event T
    public static void Raise(T @event)
    {
        //Debug.Log($"Raising Event Type {typeof(T).Name}");

        if (_disabledEvents.Contains(typeof(T))) return;
        
        foreach (var binding in _eventBindings)
        {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }
    
    //Cleanup method to remove all listeners from given Buses and remove all Disabled Events
    public static void Clear()
    {
        _eventBindings.Clear();
        _disabledEvents.Clear();
        //Debug.Log($"Cleared Bus of Event Type {typeof(T).Name}");
    }
}