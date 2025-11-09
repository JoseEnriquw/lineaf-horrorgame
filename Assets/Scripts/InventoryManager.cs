using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor centralizado del inventario del jugador.
/// 
/// RESPONSABILIDADES:
/// - Almacenar items
/// - Validar espacio disponible
/// - Equipar/Desequipar items
/// - Notificar eventos de inventario
/// 
/// FLUJO DE INTERACCIÓN:
/// 1. RaycastDetector detecta item
/// 2. RaycastDetector llama CanAddItem() - Valida espacio
/// 3. RaycastDetector llama AddItem() - Agrega a inventario
/// 4. WorldItem.OnInteract() - Se destruye
/// 5. InventoryManager dispara evento OnItemPickedUp
/// </summary>
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

        // Agregar items iniciales
        foreach (var item in startingItems)
        {
            AddItem(item);
        }
    }

    /// <summary>
    /// Valida si hay espacio para agregar un item
    /// Llamado por RaycastDetector antes de AddItem()
    /// </summary>
    public bool CanAddItem(InventoryItem item)
    {
        if (item == null)
            return false;

        if (items.Count >= maxSlots)
        {
            Debug.LogWarning("[InventoryManager] Inventario lleno - No se puede agregar item");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Agrega un item al inventario
    /// Llamado por RaycastDetector después de validar con CanAddItem()
    /// </summary>
    public bool AddItem(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogError("[InventoryManager] Intento de agregar item null");
            return false;
        }

        if (!CanAddItem(item))
        {
            return false;
        }

        items.Add(item);
        InventoryEvents.OnItemPickedUp.Invoke(item.itemID);

        Debug.Log($"[InventoryManager] ✓ {item.itemName} agregado al inventario (Slot: {items.Count})");
        return true;
    }

    /// <summary>
    /// Remueve un item del inventario
    /// Si está equipado, lo desequipa primero
    /// </summary>
    public bool RemoveItem(InventoryItem item)
    {
        if (item == null)
            return false;

        if (items.Contains(item))
        {
            // Si es el item equipado, desequiparlo primero
            if (currentEquippedItem == item)
            {
                UnequipCurrentItem();
            }

            items.Remove(item);
            Debug.Log($"[InventoryManager] ✗ {item.itemName} removido del inventario");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Equipa un item en el slot especificado
    /// Si el slot contiene el item ya equipado, lo desequipa
    /// </summary>
    public void EquipItemAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            Debug.LogWarning($"[InventoryManager] Slot inválido: {slotIndex}");
            return;
        }

        InventoryItem item = items[slotIndex];
        if (item == null)
        {
            Debug.LogWarning($"[InventoryManager] Slot {slotIndex} vacío");
            return;
        }

        // Toggle: si es el mismo slot, desequipar
        if (currentSlot == slotIndex && currentEquippedItem != null)
        {
            UnequipCurrentItem();
            return;
        }

        // Desequipar item anterior si hay uno
        if (currentEquippedItem != null)
        {
            UnequipCurrentItem();
        }

        // Equipar nuevo item
        EquipItem(item, slotIndex);
    }

    /// <summary>
    /// Equipa internamente un item
    /// </summary>
    private void EquipItem(InventoryItem item, int slotIndex)
    {
        if (item == null)
            return;

        currentEquippedItem = item;
        currentSlot = slotIndex;

        // Instanciar el prefab del item equipado si existe
        if (item.equippedPrefab != null && equipmentParent != null)
        {
            currentEquippedObject = Instantiate(item.equippedPrefab, equipmentParent);
            currentEquippedObject.transform.localPosition = Vector3.zero;
            currentEquippedObject.transform.localRotation = Quaternion.identity;
        }

        InventoryEvents.OnItemEquipped.Invoke(item.itemID);
        Debug.Log($"[InventoryManager] ⚡ {item.itemName} equipado (Slot: {slotIndex + 1})");
    }

    /// <summary>
    /// Desequipa el item actualmente equipado
    /// </summary>
    public void UnequipCurrentItem()
    {
        if (currentEquippedItem == null)
            return;

        string itemName = currentEquippedItem.itemName;
        string itemID = currentEquippedItem.itemID;

        // Destruir objeto equipado
        if (currentEquippedObject != null)
        {
            Destroy(currentEquippedObject);
            currentEquippedObject = null;
        }

        // Disparar evento
        InventoryEvents.OnItemUnequipped.Invoke(itemID);

        // Resetear estado
        currentEquippedItem = null;
        currentSlot = -1;

        Debug.Log($"[InventoryManager] ✗ {itemName} desequipado");
    }

    /// <summary>
    /// Obtiene el item actualmente equipado
    /// </summary>
    public InventoryItem GetCurrentEquippedItem() => currentEquippedItem;

    /// <summary>
    /// Obtiene la lista de todos los items en el inventario
    /// </summary>
    public List<InventoryItem> GetItems() => items;

    /// <summary>
    /// Obtiene la cantidad de items en el inventario
    /// </summary>
    public int GetItemCount() => items.Count;

    /// <summary>
    /// Obtiene espacio disponible en el inventario
    /// </summary>
    public int GetAvailableSlots() => maxSlots - items.Count;

    /// <summary>
    /// Verifica si el inventario está lleno
    /// </summary>
    public bool IsInventoryFull() => items.Count >= maxSlots;

    /// <summary>
    /// Verifica si tiene un item específico por ID
    /// </summary>
    public bool HasItem(string itemID) => items.Exists(i => i.itemID == itemID);

    /// <summary>
    /// Obtiene un item por su ID
    /// </summary>
    public InventoryItem GetItemByID(string itemID) => items.Find(i => i.itemID == itemID);
}
