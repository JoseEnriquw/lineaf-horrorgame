using UnityEngine;

/// <summary>
/// Botón del elevador que implementa IInteractable.
/// Se integra con el sistema RaycastDetector del juego para detectar interacciones.
/// 
/// FLUJO DE INTERACCIÓN:
/// 1. RaycastDetector detecta el raycast del jugador
/// 2. RaycastDetector llama a OnLookAt() - resalta el botón
/// 3. Jugador presiona E
/// 4. RaycastDetector llama a OnInteract() - presiona el botón
/// 5. RaycastDetector pierde la línea de visión
/// 6. RaycastDetector llama a OnLookAway() - restaura visual
/// </summary>
public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    [SerializeField] private ElevatorControllerEnhanced elevatorController;
    [SerializeField] private float elevatorHeight = 0f;
    [SerializeField] private bool useFixedHeight = true;
    [SerializeField] private Vector3 destinationPosition;
    
    [Header("Audio")]
    [SerializeField] private AudioSource pressSound;
    [SerializeField] private float soundPitchVariation = 0.1f;

    private bool isPressed = false;
    private const float PRESS_COOLDOWN = 1f;

    private ItemHighlight highlight;

    void Awake()
    {
        // Obtener o agregar componente de highlight
        highlight = GetComponent<ItemHighlight>();
        if (highlight == null)
        {
            highlight = gameObject.AddComponent<ItemHighlight>();
        }
    }

    private void Start()
    {
        // Buscar controlador si no está asignado
        if (elevatorController == null)
        {
            elevatorController = GameObject.Find("ElevatorSystem")?.GetComponent<ElevatorControllerEnhanced>();
            if (elevatorController == null)
            {
                Debug.LogError($"[ElevatorButton] {gameObject.name}: No se encontró ElevatorControllerEnhanced", gameObject);
            }
        }

        // Obtener AudioSource del botón
        if (pressSound == null)
        {
            pressSound = GetComponent<AudioSource>();
        }
    }

    public void OnLookAt()
    {
        if (highlight != null)
        {
            highlight.EnableHighlight();
        }
    }

    public void OnLookAway()
    {
        if (highlight != null)
        {
            highlight.DisableHighlight();
        }
    }

    /// <summary>
    /// IInteractable: Se ejecuta cuando el jugador presiona la tecla de interacción (E)
    /// </summary>
    public void OnInteract()
    {
        if (isPressed || elevatorController == null)
            return;

        isPressed = true;

        // Calcular destino
        Vector3 target = useFixedHeight
            ? new Vector3(elevatorController.Position.x, elevatorHeight, elevatorController.Position.z)
            : destinationPosition;

        // Llamar al elevador
        elevatorController.MoveTo(target);

        // Reproducir sonido con variación
        if (pressSound != null)
        {
            pressSound.pitch = Random.Range(1f - soundPitchVariation, 1f + soundPitchVariation);
            pressSound.Play();
        }

        Debug.Log($"[ElevatorButton] Botón {gameObject.name} presionado - Elevador a altura {elevatorHeight}", gameObject);

        // Cooldown para evitar spamming
        Invoke(nameof(ResetPress), PRESS_COOLDOWN);
    }

    private void ResetPress()
    {
        isPressed = false;
    }

    /// <summary>
    /// Cambiar la altura del destino dinámicamente
    /// </summary>
    public void SetDestinationHeight(float height)
    {
        elevatorHeight = height;
    }

    /// <summary>
    /// Obtener la altura actual del destino
    /// </summary>
    public float GetDestinationHeight() => elevatorHeight;
}
