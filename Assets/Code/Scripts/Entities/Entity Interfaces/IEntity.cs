using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public List<IWeakness> Weaknesses { get; }
    public void OnDamage(IWeakness damageType) { }
}


