using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador simplificado del inventario del jugador.
/// 
/// RESPONSABILIDADES:
/// - Manejar input de equipamiento (1-5, X)
/// - Mostrar UI de interacción
/// - Gestionar equipamiento de items
/// 
/// NO MANEJA:
/// - Detección de items (RaycastDetector)
/// - Recoger items (RaycastDetector + InventoryManager)
/// - Interacción con objetos (RaycastDetector)
/// 
/// FLUJO:
/// 1. RaycastDetector detecta y maneja interacciones
/// 2. PlayerInventory solo maneja equipamiento (1-5) y UI
/// 3. InventoryManager gestiona el almacenamiento de items
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RaycastDetector raycastDetector;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Input - Equipamiento")]
    [SerializeField] private KeyCode[] itemKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    [SerializeField] private KeyCode unequipKey = KeyCode.X;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

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

        if (raycastDetector == null || inventoryManager == null)
        {
            Debug.LogError("[PlayerInventory] Referencias no encontradas", gameObject);
        }
    }

    void Update()
    {
        // Actualizar UI de interacción (muestra cuando hay target)
        UpdateInteractUI();

        // Input: Interactuar (E) - Delega a RaycastDetector
        if (Input.GetKeyDown(interactKey))
        {
            if (raycastDetector != null)
            {
                raycastDetector.Interact();
            }
        }

        // Input: Equipar items (1-5)
        HandleEquipmentInput();

        // Input: Desequipar (X)
        if (Input.GetKeyDown(unequipKey))
        {
            if (inventoryManager != null)
            {
                inventoryManager.UnequipCurrentItem();
            }
        }
    }

    /// <summary>
    /// Actualiza la UI de interacción basada en si hay target visible
    /// </summary>
    private void UpdateInteractUI()
    {
        if (interactPrompt == null || raycastDetector == null)
            return;

        bool shouldShow = raycastDetector.HasTarget;

        if (interactPrompt.activeSelf != shouldShow)
        {
            interactPrompt.SetActive(shouldShow);
        }
    }

    /// <summary>
    /// Maneja el input para equipar items de los slots (1-5)
    /// </summary>
    private void HandleEquipmentInput()
    {
        if (inventoryManager == null)
            return;

        for (int i = 0; i < itemKeys.Length; i++)
        {
            if (Input.GetKeyDown(itemKeys[i]))
            {
                inventoryManager.EquipItemAtSlot(i);
            }
        }
    }
}
