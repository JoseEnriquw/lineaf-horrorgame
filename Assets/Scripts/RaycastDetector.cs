using Assets.Scripts.UI;
using UnityEngine;

/// <summary>
/// RaycastDetector centralizado para todas las interacciones del juego.
/// 
/// RESPONSABILIDADES:
/// - Detectar objetos interactuables (raycast desde cámara)
/// - Notificar eventos de look (OnLookAt, OnLookAway)
/// - Ejecutar interacciones (OnInteract)
/// - Manejar items del inventario
/// - Manejar puertas y botones
/// - Manejar objetos personalizados (IInteractable)
/// 
/// FLUJO:
/// 1. Jugador presiona E
/// 2. Este script llama a Interact()
/// 3. Valida qué tipo de objeto es (item, puerta, botón, etc)
/// 4. Ejecuta lógica específica o llama OnInteract()
/// 5. Objeto responde
/// </summary>
public class RaycastDetector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 5f;
    [SerializeField] private LayerMask detectionLayer = -1;

    [Header("Inventario")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showGizmos = true;

    private RaycastHit hit;
    private Ray ray;
    private IInteractable currentTarget;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;
    private bool hasHit = false;

    public IInteractable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;
    public RaycastHit LastHit => hit;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("[RaycastDetector] No se encontró ninguna cámara!", gameObject);
                enabled = false;
                return;
            }
            else if (showDebugLogs)
            {
                Debug.Log("[RaycastDetector] Cámara asignada automáticamente");
            }
        }

        // Auto-obtener InventoryManager si no está asignado
        if (inventoryManager == null)
        {
            inventoryManager = GetComponent<InventoryManager>();
        }

        if (showDebugLogs)
        {
            Debug.Log($"[RaycastDetector] Inicializado - Rango: {interactRange}m");
        }
    }

    void Update()
    {
        PerformRaycast();
    }

    /// <summary>
    /// Realiza el raycast desde el centro de la pantalla
    /// </summary>
    private void PerformRaycast()
    {
        if (playerCamera == null) return;

        rayOrigin = playerCamera.transform.position;
        rayDirection = playerCamera.transform.forward;
        ray = new Ray(rayOrigin, rayDirection);

        bool rayHit = detectionLayer.value == -1
            ? Physics.Raycast(rayOrigin, rayDirection, out hit, interactRange)
            : Physics.Raycast(rayOrigin, rayDirection, out hit, interactRange, detectionLayer);

        if (rayHit)
        {
            hasHit = true;

            if (showDebugLogs)
            {
                Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.green);
            }

            // Buscar IInteractable en el objeto golpeado
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable == null && hit.collider.transform.parent != null)
            {
                interactable = hit.collider.transform.parent.GetComponent<IInteractable>();
            }

            if (interactable != null)
            {
                // Nuevo target detectado
                if (currentTarget != interactable)
                {
                    if (showDebugLogs)
                    {
                        string targetType = interactable is IInteractableItems ? "Item" : "Interactable";
                        Debug.Log($"[RaycastDetector] Detectado: {hit.collider.gameObject.name} ({targetType})");
                    }

                    // Notificar al anterior que ya no lo miramos
                    currentTarget?.OnLookAway();

                    // Establecer nuevo target
                    currentTarget = interactable;
                    currentTarget.OnLookAt();
                }
                return;
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning($"[RaycastDetector] Raycast tocó {hit.collider.gameObject.name} pero NO tiene IInteractable!");
            }



        }
        else
        {
            hasHit = false;

            if (showDebugLogs)
            {
                Debug.DrawRay(rayOrigin, rayDirection * interactRange, Color.red);
            }
        }

        // No hay target o se perdió de vista
        if (currentTarget != null)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[RaycastDetector] Perdido target anterior");
            }
            currentTarget.OnLookAway();
            currentTarget = null;
        }
    }

    /// <summary>
    /// Ejecuta la interacción con el objeto actual
    /// Este es el HUB CENTRAL de todas las interacciones
    /// </summary>
    public void Interact()
    {
        if (currentTarget == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("[RaycastDetector] Intento de interacción sin target");
            return;
        }

        // Caso 1: Es un item del inventario
        if (currentTarget is IInteractableItems itemTarget)
        {
            HandleItemInteraction(itemTarget);
        }
        // Caso 2: Es otro objeto interactuable (puertas, botones, etc)
        else if (currentTarget is IInteractable interactable)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[RaycastDetector] Interactuando con: {GetCurrentTargetName()}");
            }
            interactable.OnInteract();
        }
    }

    /// <summary>
    /// Maneja la interacción con items del mundo
    /// Valida espacio en inventario antes de recoger
    /// </summary>
    private void HandleItemInteraction(IInteractableItems itemTarget)
    {
        if (inventoryManager == null)
        {
            if (showDebugLogs)
                Debug.LogError("[RaycastDetector] InventoryManager no asignado. No se puede recoger items.");
            return;
        }

        // Obtener datos del item
        InventoryItem itemData = itemTarget.GetItemData();
        if (itemData == null)
        {
            if (showDebugLogs)
                Debug.LogError("[RaycastDetector] Item no tiene InventoryItem data");
            return;
        }

        // Validar que hay espacio en inventario
        if (!inventoryManager.CanAddItem(itemData))
        {
            if (showDebugLogs)
                Debug.LogWarning($"[RaycastDetector] No hay espacio en inventario para: {itemData.itemName}");
            return;
        }

        // Intentar agregar item al inventario
        if (inventoryManager.AddItem(itemData))
        {
            if (showDebugLogs)
                Debug.Log($"[RaycastDetector] Item recogido: {itemData.itemName}");

            // Si se agregó exitosamente, ejecutar OnInteract (destruye el objeto)
            itemTarget.OnInteract();
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"[RaycastDetector] Fallo al agregar item: {itemData.itemName}");
        }
    }

    /// <summary>
    /// Verifica si hay un target visible
    /// </summary>
    public bool IsLookingAtInteractable()
    {
        return currentTarget != null;
    }

    /// <summary>
    /// Obtiene el nombre del objeto mirando
    /// </summary>
    public string GetCurrentTargetName()
    {
        if (currentTarget is MonoBehaviour mb)
        {
            return mb.gameObject.name;
        }
        return "Desconocido";
    }

    /// <summary>
    /// Obtiene la distancia al objeto actual
    /// </summary>
    public float GetDistanceToTarget()
    {
        if (currentTarget != null && hasHit)
        {
            return hit.distance;
        }
        return -1f;
    }

    /// <summary>
    /// Cambiar rango de interacción dinámicamente
    /// </summary>
    public void SetInteractRange(float newRange)
    {
        interactRange = Mathf.Max(0.1f, newRange);
    }

    /// <summary>
    /// Cambiar capa de detección dinámicamente
    /// </summary>
    public void SetDetectionLayer(LayerMask newLayer)
    {
        detectionLayer = newLayer;
    }

    // ===== DEBUGGING Y VISUALIZACIÓN =====

    void OnDrawGizmos()
    {
        if (!showGizmos || playerCamera == null) return;

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        Gizmos.color = HasTarget ? Color.green : Color.yellow;
        Gizmos.DrawRay(origin, direction * interactRange);

        if (HasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(origin + direction * GetDistanceToTarget(), 0.2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos || playerCamera == null) return;

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        Gizmos.color = HasTarget ? Color.green : Color.cyan;
        Gizmos.DrawLine(origin, origin + direction * interactRange);
    }
}
