using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RaycastDetector raycastDetector;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Teclas")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode unequipKey = KeyCode.X;
    [SerializeField]
    private KeyCode[] itemKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    [Header("UI (Opcional)")]
    [SerializeField] private GameObject interactPrompt;

    void Start()
    {
        // Auto-obtener componentes si no están asignados
        if (raycastDetector == null)
        {
            raycastDetector = GetComponent<RaycastDetector>();
        }

        if (inventoryManager == null)
        {
            inventoryManager = GetComponent<InventoryManager>();
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Actualizar UI de interacción
        UpdateInteractUI();

        // Input: Interactuar (recoger)
        if (Input.GetKeyDown(interactKey))
        {
            TryPickupItem();
        }

        // Input: Equipar items
        for (int i = 0; i < itemKeys.Length; i++)
        {
            if (Input.GetKeyDown(itemKeys[i]))
            {
                inventoryManager.EquipItemAtSlot(i);
            }
        }

        // Input: Desequipar
        if (Input.GetKeyDown(unequipKey))
        {
            inventoryManager.UnequipCurrentItem();
        }
    }

    private void UpdateInteractUI()
    {
        if (interactPrompt == null) return;

        bool shouldShow = raycastDetector.HasTarget;

        if (interactPrompt.activeSelf != shouldShow)
        {
            interactPrompt.SetActive(shouldShow);
        }
    }

    private void TryPickupItem()
    {
        if (!raycastDetector.HasTarget) return;

        IInteractable target = raycastDetector.CurrentTarget;
        InventoryItem itemData = target.GetItemData();

        if (itemData != null && inventoryManager.AddItem(itemData))
        {
            raycastDetector.Interact();
        }
    }
}
