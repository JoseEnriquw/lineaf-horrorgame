using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private Transform equipmentParent;

    [Header("Items Iniciales")]
    [SerializeField] private List<InventoryItem> startingItems = new List<InventoryItem>();

    private List<InventoryItem> items = new List<InventoryItem>();
    private InventoryItem currentEquippedItem;
    private GameObject currentEquippedObject;
    private int currentSlot = -1;

    void Start()
    {
        if (equipmentParent == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                equipmentParent = mainCam.transform;
            }
        }

        foreach (var item in startingItems)
        {
            AddItem(item);
        }
    }

    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        items.Add(item);
        InventoryEvents.OnItemPickedUp.Invoke(item.itemID);

        Debug.Log($"✓ {item.itemName} agregado (Presiona {items.Count} para equipar)");
        return true;
    }

    public bool RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            if (currentEquippedItem == item)
            {
                UnequipCurrentItem();
            }

            items.Remove(item);
            return true;
        }

        return false;
    }

    public void EquipItemAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        // Toggle si es el mismo slot
        if (currentSlot == slotIndex && currentEquippedItem != null)
        {
            UnequipCurrentItem();
            return;
        }

        if (currentEquippedItem != null)
        {
            UnequipCurrentItem();
        }

        EquipItem(items[slotIndex], slotIndex);
    }

    private void EquipItem(InventoryItem item, int slotIndex)
    {
        currentEquippedItem = item;
        currentSlot = slotIndex;

        if (item.equippedPrefab != null && equipmentParent != null)
        {
            currentEquippedObject = Instantiate(item.equippedPrefab, equipmentParent);
            currentEquippedObject.transform.localPosition = Vector3.zero;
            currentEquippedObject.transform.localRotation = Quaternion.identity;
        }

        InventoryEvents.OnItemEquipped.Invoke(item.itemID);
        Debug.Log($"⚡ {item.itemName} equipado");
    }

    public void UnequipCurrentItem()
    {
        if (currentEquippedItem == null) return;

        string itemID = currentEquippedItem.itemID;

        if (currentEquippedObject != null)
        {
            Destroy(currentEquippedObject);
            currentEquippedObject = null;
        }

        InventoryEvents.OnItemUnequipped.Invoke(itemID);

        currentEquippedItem = null;
        currentSlot = -1;
    }

    public InventoryItem GetCurrentEquippedItem() => currentEquippedItem;
    public List<InventoryItem> GetItems() => items;
    public bool HasItem(string itemID) => items.Exists(i => i.itemID == itemID);
}
