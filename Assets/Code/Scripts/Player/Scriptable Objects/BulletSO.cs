using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Bullet Type", order = 1)]
public class BulletSO : ScriptableObject
{
    public string bulletTypeName;
    public Color bulletColor;
    public WeakTypes weakness;
}
