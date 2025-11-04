using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string itemID;
    public Sprite icon;
    public GameObject worldPrefab;
    public GameObject equippedPrefab;

    [TextArea(3, 5)]
    public string description;
}
