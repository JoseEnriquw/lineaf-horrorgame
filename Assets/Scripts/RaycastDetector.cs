using UnityEngine;

public class RaycastDetector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask itemLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private RaycastHit hit;
    private Ray ray;
    private IInteractable currentTarget;

    public IInteractable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("RaycastDetector: No se encontró ninguna cámara!");
            }
            else
            {
                Debug.Log($"RaycastDetector: Cámara asignada automáticamente");
            }
        }

        // Verificar que el LayerMask no esté en 0 (Nothing)
        if (itemLayer.value == 0)
        {
            Debug.LogWarning("RaycastDetector: Item Layer está en 'Nothing'. Asigna la capa 'Items'!");
        }

        Debug.Log($"RaycastDetector inicializado - Rango: {interactRange}m, Layer: {itemLayer.value}");
    }

    void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        if (playerCamera == null) return;

        // Raycast desde el centro de la pantalla
        ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, interactRange, itemLayer))
        {
            if (showDebugLogs)
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            }

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Nuevo target detectado
                if (currentTarget != interactable)
                {
                    if (showDebugLogs)
                    {
                        Debug.Log($"Raycast detectó: {hit.collider.gameObject.name}");
                    }

                    // Notificar al anterior que ya no lo miramos
                    if (currentTarget != null)
                    {
                        currentTarget.OnLookAway();
                    }

                    // Establecer nuevo target
                    currentTarget = interactable;
                    currentTarget.OnLookAt();
                }
                return;
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning($"Raycast tocó {hit.collider.gameObject.name} pero no tiene IInteractable!");
            }
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);
            }
        }

        // No hay target
        if (currentTarget != null)
        {
            currentTarget.OnLookAway();
            currentTarget = null;
        }
    }

    public void Interact()
    {
        if (currentTarget != null)
        {
            currentTarget.OnInteract();
            currentTarget = null;
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null) return;

        Gizmos.color = HasTarget ? Color.green : Color.yellow;
        Ray debugRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Gizmos.DrawRay(debugRay.origin, debugRay.direction * interactRange);
    }
}
