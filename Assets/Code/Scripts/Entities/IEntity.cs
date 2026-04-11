using System.Collections.Generic;

public interface IEntity
{
    public void InitialiseWeaknesses();
    public void RemoveNulledWeakness(Weakness weakness);
    public List<Weakness> Weaknesses { get; }
    public void OnShot(Weakness weakness, WeakTypes damageType);
}


