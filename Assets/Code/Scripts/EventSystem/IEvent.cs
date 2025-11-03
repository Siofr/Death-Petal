/// <summary>
/// Tag interface for any events
/// For new Events make sure to create structs that inherit from the IEvent interface
///
/// Example:
/// public struct PlayerEvent: IEvent
/// {
///     public Vector3 position;
/// }
/// </summary>
public interface IEvent {}