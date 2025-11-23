using System.Collections.Generic;

public interface IEntity
{
    public void InitialiseWeaknesses();
    public List<Weakness> Weaknesses { get; }
    public void OnShot(Weakness weakness, WeakTypes damageType);
}


