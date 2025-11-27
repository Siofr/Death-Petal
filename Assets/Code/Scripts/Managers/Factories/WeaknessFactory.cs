using UnityEngine;

public class WeaknessFactory: MonoBehaviour
{
    public GameObject weaknessPrefab;
    
    public Weakness CreateWeakness(EntityBase entity, WeakTypes weakType)
    {
        var weaknessContainer = entity.transform.Find("Weaknesses");

        var objectInstance = Instantiate(weaknessPrefab, weaknessContainer);

        var product = objectInstance.GetComponent<Weakness>();
        product.Initialize(weakType);

        return product;
    }
}