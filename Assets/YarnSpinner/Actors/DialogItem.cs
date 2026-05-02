using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "DialogItem", menuName = "Scriptable Objects/DialogItem")]
public class DialogItem : ScriptableObject
{
    [Header("Item Details")]
    public int itemID;

    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public EventReference ItemEnterSFX;
}
