using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Puerta de elevador que implementa IInteractable e IElevatorDoor.
/// Soporta dos tipos: Articulada (Hinged) y Acordeón (Accordion)
/// 
/// FLUJO DE INTERACCIÓN:
/// 1. RaycastDetector detecta el raycast del jugador
/// 2. RaycastDetector llama a OnLookAt() - activa ItemHighlight
/// 3. Jugador presiona E
/// 4. RaycastDetector llama a OnInteract() - alterna puerta (abre/cierra)
/// 5. Puerta anima y reproduce sonidos
/// 6. RaycastDetector pierde la línea de visión
/// 7. RaycastDetector llama a OnLookAway() - desactiva ItemHighlight
/// 
/// ANIMACIÓN:
/// - Hinged: Rotación basada en ángulos
/// - Accordion: Blendshape + MeshCollider rebakeado
/// </summary>
public class ElevatorDoor : MonoBehaviour, IInteractable, IElevatorDoor
{
    [System.Serializable]
    public enum DoorType { Hinged, Accordion }

    [System.Serializable]
    public enum MovementType { Normal, CustomCurve }

    [Header("Tipo de Puerta")]
    [SerializeField] private DoorType doorType = DoorType.Hinged;
    public ElevatorDoor PairedDoor;
    
    [Header("Movimiento")]
    [SerializeField] private MovementType movementType = MovementType.Normal;

    [Header("Configuración Hinged (Articulada)")]
    [SerializeField] private float openAngle = 100f;
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private bool isRightDoor = false;
    [SerializeField] private AnimationCurve angleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Velocidad")]
    [SerializeField] private float animationSpeed = 0.1f;
    [SerializeField] private bool randomizeSpeed = true;
    [SerializeField] private float minRandomSpeed = 0.03f;
    [SerializeField] private float maxRandomSpeed = 0.15f;

    [Header("Sonidos")]
    [SerializeField] private List<AudioSource> openingSounds = new();
    [SerializeField] private List<AudioSource> closingSounds = new();

    // ===== ESTADO =====
    private DoorState currentState = DoorState.FullyClosed;
    private bool locked = false;
    private bool isOpening = false;
    private bool isClosing = false;

    // ===== ANIMACIÓN HINGED =====
    private float currentZAngle;
    private float destinationZAngle;
    private Vector3 originalLocalRotation;
    private float angleCurveStartTime;
    private float curveSpeed = 1f;
    private float currentSpeed;

    // ===== ANIMACIÓN ACCORDION =====
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private float currentBlendShapeWeight = 0f;
    private MeshCollider meshCollider;

    // ===== REFERENCIAS =====
    public DoorState CurrentState => currentState;
    public bool IsLocked => locked;
    public bool IsFullyClosed => currentState == DoorState.FullyClosed;
    public bool IsFullyOpen => currentState == DoorState.FullyOpen;

    private ItemHighlight highlight;

    private void Awake()
    {
        // Obtener o agregar ItemHighlight para resalte visual
        highlight = GetComponent<ItemHighlight>();
        if (highlight == null)
        {
            highlight = gameObject.AddComponent<ItemHighlight>();
        }

        // Inicializar según tipo de puerta
        if (doorType == DoorType.Accordion)
        {
            InitializeAccordionDoor();
        }
        else
        {
            InitializeHingedDoor();
        }

        // Buscar sonidos en carpetas hijas
        FindAudioSources();
    }

    private void Start()
    {
        currentState = DoorState.FullyClosed;
        isOpening = false;
        isClosing = false;
    }

    private void FixedUpdate()
    {
        // No animar si está bloqueada
        if (locked)
            return;

        UpdateDoorAnimation();
    }

    /// <summary>
    /// Inicializa puerta tipo Accordion (acordeón)
    /// </summary>
    private void InitializeAccordionDoor()
    {
        TryGetComponent(out skinnedMeshRenderer);
        
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        RebakeMesh();
    }

    /// <summary>
    /// Inicializa puerta tipo Hinged (articulada)
    /// </summary>
    private void InitializeHingedDoor()
    {
        originalLocalRotation = transform.localEulerAngles;
        currentZAngle = closedAngle;
        destinationZAngle = closedAngle;
    }

    /// <summary>
    /// Actualiza la animación de la puerta según su tipo
    /// </summary>
    private void UpdateDoorAnimation()
    {
        if (doorType == DoorType.Accordion)
        {
            UpdateAccordionAnimation();
        }
        else
        {
            UpdateHingedAnimation();
        }
    }

    /// <summary>
    /// Actualiza animación para puerta acordeón usando blendshape
    /// </summary>
    private void UpdateAccordionAnimation()
    {
        if (isOpening)
        {
            currentBlendShapeWeight = Mathf.Lerp(currentBlendShapeWeight, 100f, currentSpeed);
            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(0, currentBlendShapeWeight);
            }

            if (currentBlendShapeWeight > 99f)
            {
                CompleteOpening();
            }
        }
        else if (isClosing)
        {
            currentBlendShapeWeight = Mathf.Lerp(currentBlendShapeWeight, 0f, currentSpeed);
            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(0, currentBlendShapeWeight);
            }

            if (currentBlendShapeWeight < 1f)
            {
                CompleteClosing();
            }
        }
    }

    /// <summary>
    /// Actualiza animación para puerta articulada (rotación)
    /// </summary>
    private void UpdateHingedAnimation()
    {
        if (!isOpening && !isClosing)
            return;

        if (movementType == MovementType.Normal)
        {
            currentZAngle = Mathf.Lerp(currentZAngle, destinationZAngle, 0.1f);

            if (Mathf.Abs(currentZAngle - destinationZAngle) < 0.1f)
            {
                if (isOpening)
                    CompleteOpening();
                else if (isClosing)
                    CompleteClosing();
            }
        }
        else
        {
            float elapsedTime = (Time.time - angleCurveStartTime) * curveSpeed;
            currentZAngle = angleCurve.Evaluate(elapsedTime) * (isRightDoor ? -1 : 1);

            if (currentZAngle < 0.1f && !isRightDoor && elapsedTime > 1f)
                CompleteClosing();
            if (currentZAngle > -0.1f && isRightDoor && elapsedTime > 1f)
                CompleteClosing();
        }

        transform.localEulerAngles = new Vector3(
            originalLocalRotation.x,
            originalLocalRotation.y,
            currentZAngle
        );
    }

    /// <summary>
    /// Completa el proceso de apertura
    /// </summary>
    private void CompleteOpening()
    {
        isOpening = false;
        currentState = DoorState.FullyOpen;

        if (doorType == DoorType.Accordion && meshCollider != null)
        {
            RebakeMesh();
            meshCollider.enabled = true;
        }

        StopAudio(openingSounds);
        RandomizeAndPlay(closingSounds);

        Debug.Log($"[ElevatorDoor] {gameObject.name} abierta completamente", gameObject);
    }

    /// <summary>
    /// Completa el proceso de cierre
    /// </summary>
    private void CompleteClosing()
    {
        isClosing = false;
        currentState = DoorState.FullyClosed;

        if (doorType == DoorType.Accordion)
        {
            RebakeMesh();
        }

        StopAudio(openingSounds);
        RandomizeAndPlay(closingSounds);

        Debug.Log($"[ElevatorDoor] {gameObject.name} cerrada completamente", gameObject);
    }

    /// <summary>
    /// Abre la puerta (y su pareja si existe)
    /// </summary>
    public void Open()
    {
        if (locked || currentState == DoorState.FullyOpen)
            return;

        isOpening = true;
        isClosing = false;
        currentState = DoorState.Moving;
        currentSpeed = randomizeSpeed ? Random.Range(minRandomSpeed, maxRandomSpeed) : animationSpeed;

        if (doorType == DoorType.Hinged)
        {
            destinationZAngle = openAngle;
        }

        if (doorType == DoorType.Accordion && meshCollider != null)
        {
            meshCollider.enabled = false;
        }

        DoorStartsOpening();

        // Abrir puerta del par si existe
        if (PairedDoor != null && PairedDoor.IsFullyClosed)
        {
            PairedDoor.Open();
        }

        angleCurveStartTime = Time.time;
        curveSpeed = Random.Range(0.9f, 1.2f);
    }

    /// <summary>
    /// Cierra la puerta (y su pareja si existe)
    /// </summary>
    public void Close()
    {
        if (locked || currentState == DoorState.FullyClosed)
            return;

        isClosing = true;
        isOpening = false;
        currentState = DoorState.Moving;
        currentSpeed = randomizeSpeed ? Random.Range(0.1f, 0.2f) : animationSpeed;

        if (doorType == DoorType.Hinged)
        {
            destinationZAngle = closedAngle;
        }

        DoorStartsClosing();

        // Cerrar puerta del par si existe
        if (PairedDoor != null && !PairedDoor.isClosing)
        {
            PairedDoor.Close();
        }
    }

    /// <summary>
    /// Alterna estado de la puerta (abre si está cerrada, cierra si está abierta)
    /// </summary>
    public void Toggle()
    {
        if (currentState == DoorState.FullyClosed)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    /// Bloquea o desbloquea la puerta
    /// </summary>
    public void SetLocked(bool isLocked)
    {
        locked = isLocked;
    }

    // ===== IMPLEMENTACIÓN IINTERACTABLE =====

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
    /// Se ejecuta cuando el jugador presiona E sobre la puerta
    /// </summary>
    public void OnInteract()
    {
        if (!locked)
        {
            Toggle();
            Debug.Log($"[ElevatorDoor] {gameObject.name} interactuada por jugador", gameObject);
        }
    }

    // ===== MÉTODOS PRIVADOS =====

    /// <summary>
    /// Rebakea la malla del colisionador para puertas acordeón
    /// Necesario para que el colisionador se actualice con el blendshape
    /// </summary>
    private void RebakeMesh()
    {
        if (doorType != DoorType.Accordion || meshCollider == null || skinnedMeshRenderer == null)
            return;

        try
        {
            Mesh bakeMesh = new();
            skinnedMeshRenderer.BakeMesh(bakeMesh, false);
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = bakeMesh;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ElevatorDoor] Error rebakeando malla: {e.Message}", gameObject);
        }
    }

    /// <summary>
    /// Busca automáticamente carpetas de sonidos hijas
    /// </summary>
    private void FindAudioSources()
    {
        openingSounds.Clear();
        closingSounds.Clear();

        Transform openingFolder = transform.Find("Opening Sounds");
        if (openingFolder != null)
        {
            foreach (AudioSource audio in openingFolder.GetComponentsInChildren<AudioSource>())
            {
                openingSounds.Add(audio);
            }
        }

        Transform closingFolder = transform.Find("Closing Sounds");
        if (closingFolder != null)
        {
            foreach (AudioSource audio in closingFolder.GetComponentsInChildren<AudioSource>())
            {
                closingSounds.Add(audio);
            }
        }

        if (openingSounds.Count > 0)
            Debug.Log($"[ElevatorDoor] {gameObject.name}: Encontrados {openingSounds.Count} sonidos de apertura", gameObject);
        if (closingSounds.Count > 0)
            Debug.Log($"[ElevatorDoor] {gameObject.name}: Encontrados {closingSounds.Count} sonidos de cierre", gameObject);
    }

    private void DoorStartsOpening()
    {
        RandomizeAndPlay(openingSounds);
    }

    private void DoorStartsClosing()
    {
        RandomizeAndPlay(openingSounds);
    }

    /// <summary>
    /// Reproduce un sonido aleatorio de una lista con variación de pitch
    /// </summary>
    private void RandomizeAndPlay(List<AudioSource> audioList)
    {
        if (audioList.Count == 0)
            return;

        AudioSource audio = audioList[Random.Range(0, audioList.Count)];
        audio.pitch = Random.Range(0.8f, 1.3f);
        audio.Play();
    }

    /// <summary>
    /// Detiene todos los sonidos de una lista
    /// </summary>
    private void StopAudio(List<AudioSource> audioList)
    {
        foreach (AudioSource audio in audioList)
        {
            if (audio.isPlaying)
                audio.Stop();
        }
    }
}
