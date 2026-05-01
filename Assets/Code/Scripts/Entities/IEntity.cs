using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEntity
{
    public virtual async Task InitialiseWeaknesses() { }

    public void RemoveNulledWeakness(Weakness weakness);
    public List<Weakness> Weaknesses { get; }
    public void OnShot(Weakness weakness, WeakTypes damageType);
}


