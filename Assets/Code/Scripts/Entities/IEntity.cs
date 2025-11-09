using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public List<Weakness> Weaknesses { get; }
    public void OnDamage(Weakness damageType) { }
}


