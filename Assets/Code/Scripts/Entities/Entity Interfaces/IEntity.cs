using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public List<IWeakness> Weaknesses { get; set; }
}

[Flags]
public enum WeakTypes {
    PLAYER,
    RED,
    BLUE,
    GREEN
}

public interface IWeakness
{
    public WeakTypes WeakType { get; } 
}
