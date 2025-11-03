using System;

/// <summary>
/// Event Bindings:
///
/// Used to add listeners in object instances to an event
/// Listeners can either add a parameterless method to the OnNoArgsEvent delegate
/// Or add a method that takes a type T event parameter to the OnEvent delegate
/// </summary>
/// <typeparam name="T"> T refers to an Event paramater </typeparam>
public class EventBindings<T> : IEventBindings<T> where T : IEvent
{
    private Action<T> _onEvent = (eventType) => { };
    private Action _onEventNoArgs = () => { };

    public Action<T> OnEvent
    {
        get => _onEvent;
        set => _onEvent = value;
    }

    public Action OnEventNoArgs
    {
        get => _onEventNoArgs;
        set => _onEventNoArgs = value;
    }
    
    //Event Binding Constructors
    public EventBindings(Action<T> onEvent) => this._onEvent = onEvent;
    public EventBindings(Action onEventNoArgs) => this._onEventNoArgs = onEventNoArgs;
    
    //Functions to add more methods to the binding delegate
    public void Add(Action<T> value) => _onEvent += value;
    public void Remove(Action<T> value) => _onEvent -= value;

    public void Add(Action value) => _onEventNoArgs += value;
    public void Remove(Action value) => _onEventNoArgs -= value;
}