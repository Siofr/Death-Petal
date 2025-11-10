using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapon Data", order = 2)]
public class PlayerWeaponSO : ScriptableObject
{
    public string gunName;
    public int maxBullets;
}
