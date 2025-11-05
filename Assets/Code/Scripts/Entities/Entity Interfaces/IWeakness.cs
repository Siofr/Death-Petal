using System;

[Flags]
public enum WeakTypes {
    NONE,
    RED,
    BLUE,
    GREEN,
    PLAYER = Int32.MaxValue
}

public interface IWeakness
{
    public WeakTypes WeakType { get; } 
}