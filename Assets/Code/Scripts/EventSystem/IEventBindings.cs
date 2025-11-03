using System;

/// <summary>
/// Interface for EventBindings
///
/// Forces to have a property for delegates with or w/o arguments
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEventBindings<T>
{
    Action<T> OnEvent { get; set; }
    Action OnEventNoArgs { get; set; }
}