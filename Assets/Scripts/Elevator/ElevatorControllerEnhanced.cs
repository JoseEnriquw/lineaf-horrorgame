using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Versión mejorada del ElevatorController con características de horror y efectos visuales.
/// Agrega parpadeo de luces, temblores, y efectos de sonido avanzados.
/// Compatible con la arquitectura existente.
/// </summary>
public class ElevatorControllerEnhanced : MonoBehaviour, IElevator
{
    [SerializeField] private ElevatorConfiguration config;
    [SerializeField] private GameObject elevatorGameObject;

    // Referencias a puertas
    private List<ElevatorDoor> internalDoors = new List<ElevatorDoor>();
    private List<ElevatorDoor> externalDoors = new List<ElevatorDoor>();

    // Estado del movimiento
    private Vector3 currentDestination;
    private bool isMoving = false;
    private int moveDirection = 0;
    private float currentSpeed;
    private bool isBreaking = false;
    private bool hasReachedDestination = false;

    [Header("Efectos Visuales")]
    [SerializeField] private bool enableLightFlickering = true;
    [SerializeField] private FlickeringFrequency lightFlickeringLevel = FlickeringFrequency.AlmostUnnoticeable;
    [SerializeField] private bool enableElevatorShaking = true;
    [SerializeField] private ShakingIntensity shakingLevel = ShakingIntensity.AlmostUnnoticeable;
    [SerializeField] private float shakeIntensityOnStart = 0.5f;
    [SerializeField] private float shakeIntensityOnStop = 0.3f;

    public enum FlickeringFrequency { NoFlickering, AlmostUnnoticeable, Annoying, Heavy, Nightmare }
    public enum ShakingIntensity { NoShaking, AlmostUnnoticeable, Annoying, Heavy, Nightmare }

    [Header("Luces")]
    [SerializeField] private GameObject elevatorLightsContainer;
    [SerializeField] private List<Material> lightMaterials = new List<Material>();
    [SerializeField] private Color lightColorOn = new Color(1, 1, 1, 1);
    [SerializeField] private Color lightColorOff = new Color(0.2f, 0.2f, 0.2f, 1);
    [SerializeField] private float lightIntensity = 1f;

    [Header("Sonidos")]
    [SerializeField] private AudioSource travelSound;
    [SerializeField] private AudioSource arrivalSound;
    [SerializeField] private AudioSource emergencyStopSound;
    [SerializeField] private AudioSource brakesSound;
    [SerializeField] private List<AudioSource> flickeringSounds = new List<AudioSource>();

    [Header("Jugador")]
    [SerializeField] private Transform player;
    [SerializeField] private bool preventPlayerBouncing = true;
    [SerializeField] private float distanceToFloor = 1f;
    [SerializeField] private LayerMask floorLayer;

    // Variables de efectos
    private float nextFlickerTime = 1f;
    private float nextShakeTime = 1f;
    private float currentShakeMagnitude = 0;
    private Quaternion originalElevatorRotation;
    private bool hasShownPlayerWarning = false;

    public Vector3 Position => elevatorGameObject.transform.localPosition;
    public bool IsMoving => isMoving;

    // Eventos
    public event System.Action OnElevatorStartMoving;
    public event System.Action OnElevatorStopped;
    public event System.Action OnDestinationReached;
    public event System.Action OnEmergencyStop;
    public event System.Action<float> OnShaking;
    public event System.Action<bool> OnLightFlicker;

    private void OnEnable()
    {
        ValidateConfiguration();
    }

    private void Start()
    {
        GatherDoors();
        currentDestination = elevatorGameObject.transform.position;
        originalElevatorRotation = elevatorGameObject.transform.rotation;

        // Buscar jugador si no está asignado
        if (preventPlayerBouncing && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void ValidateConfiguration()
    {
        if (config == null)
        {
            Debug.LogError("ElevatorController_Enhanced requiere una configuración.", gameObject);
        }

        if (elevatorGameObject == null)
        {
            Debug.LogError("ElevatorControllerEnhanced requiere que asignes el GameObject del elevador en el Inspector.", gameObject);
        }
    }

    private void GatherDoors()
    {
        internalDoors.Clear();
        externalDoors.Clear();

        ElevatorDoor[] allDoors = GetComponentsInChildren<ElevatorDoor>();

        foreach (ElevatorDoor door in allDoors)
        {
            if (door.CompareTag("ElevatorInternalDoor"))
            {
                internalDoors.Add(door);
            }
            else if (door.CompareTag("ElevatorExternalDoor"))
            {
                externalDoors.Add(door);
            }
        }

        PairDoors(internalDoors);
        PairDoors(externalDoors);
    }

    private void PairDoors(List<ElevatorDoor> doors)
    {
        for (int i = 0; i < doors.Count - 1; i += 2)
        {
            doors[i].PairedDoor = doors[i + 1];
            doors[i + 1].PairedDoor = doors[i];
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        UpdateElevatorMovement();
        HandlePlayerMovement();
    }

    private void Update()
    {
        if (enableLightFlickering && Time.time > nextFlickerTime)
        {
            ApplyLightFlickering();
        }

        if (enableElevatorShaking && Time.time > nextShakeTime)
        {
            ApplyElevatorShaking();
        }

        if (enableElevatorShaking && currentShakeMagnitude > 0.01f)
        {
            ApplyShakeRotation();
            currentShakeMagnitude *= 0.9f;
        }
    }

    private void UpdateElevatorMovement()
    {
        float movementAmount = moveDirection * currentSpeed * Time.fixedDeltaTime;
        elevatorGameObject.transform.localPosition += Vector3.up * movementAmount;

        if (config.SlowDownNearDestination)
        {
            CheckAndApplySlowdown();
        }

        if (HasReachedDestination())
        {
            CompleteMovement();
        }
    }

    private void HandlePlayerMovement()
    {
        if (!preventPlayerBouncing || player == null) return;

        if (Physics.Raycast(player.position, -Vector3.up, distanceToFloor, floorLayer))
        {
            player.position += Vector3.up * (moveDirection * currentSpeed * Time.fixedDeltaTime);
        }
        else if (!hasShownPlayerWarning)
        {
            hasShownPlayerWarning = true;
            Debug.LogWarning("Configura el Layer 'ElevatorFloor' en tu suelo para evitar que el jugador rebote.", gameObject);
        }
    }

    private void CheckAndApplySlowdown()
    {
        float distanceToDestination = Mathf.Abs(elevatorGameObject.transform.localPosition.y - currentDestination.y);

        if (distanceToDestination < 1f && !isBreaking)
        {
            isBreaking = true;
            if (brakesSound != null)
            {
                brakesSound.volume = 0.7f;
                brakesSound.Play();
            }
        }

        if (isBreaking)
        {
            currentSpeed = Mathf.Clamp(currentSpeed * (1 - config.SlowingDownEffect), 0.002f, 99f);
        }
    }

    private bool HasReachedDestination()
    {
        return (moveDirection == 1 && elevatorGameObject.transform.localPosition.y >= currentDestination.y) ||
               (moveDirection == -1 && elevatorGameObject.transform.localPosition.y <= currentDestination.y);
    }

    private void CompleteMovement()
    {
        isMoving = false;
        isBreaking = false;
        currentSpeed = 0;
        hasReachedDestination = true;

        if (travelSound != null) travelSound.Stop();
        if (arrivalSound != null) arrivalSound.Play();

        currentShakeMagnitude = shakeIntensityOnStop;

        OnDestinationReached?.Invoke();
        OnElevatorStopped?.Invoke();

        // Abre puertas externas
        foreach (ElevatorDoor door in externalDoors)
        {
            door.SetLocked(false);
            door.Open();
        }
    }

    private bool CheckSafetyConditions()
    {
        if (isMoving) return false;

        if (config.InternalDoorsMustBeClosed)
        {
            foreach (ElevatorDoor door in internalDoors)
            {
                if (door.CurrentState != DoorState.FullyClosed)
                    return false;
            }
        }

        if (config.ExternalDoorsMustBeClosed)
        {
            foreach (ElevatorDoor door in externalDoors)
            {
                if (door.CurrentState != DoorState.FullyClosed)
                    return false;
            }
        }

        return true;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (!CheckSafetyConditions()) return;

        currentDestination = targetPosition;
        moveDirection = (int)Mathf.Sign(targetPosition.y - elevatorGameObject.transform.localPosition.y);

        if (moveDirection == 0) return;

        isMoving = true;
        isBreaking = false;
        hasReachedDestination = false;
        currentSpeed = config.UnitsPerSecond;
        currentShakeMagnitude = shakeIntensityOnStart;

        if (travelSound != null)
        {
            travelSound.volume = 1f;
            travelSound.pitch = 1f;
            travelSound.Play();
        }

        // Bloquea puertas externas
        foreach (ElevatorDoor door in externalDoors)
        {
            door.SetLocked(true);
        }

        OnElevatorStartMoving?.Invoke();
    }

    public void Stop()
    {
        if (!isMoving) return;

        isMoving = false;
        currentSpeed = 0;

        if (travelSound != null) travelSound.Stop();
        if (emergencyStopSound != null) emergencyStopSound.Play();

        currentShakeMagnitude = shakeIntensityOnStop * 2f;

        OnEmergencyStop?.Invoke();
        OnElevatorStopped?.Invoke();
    }

    private void ApplyLightFlickering()
    {
        if (elevatorLightsContainer == null) return;

        float offDuration = 0, onDuration = 0;

        switch (lightFlickeringLevel)
        {
            case FlickeringFrequency.NoFlickering:
                nextFlickerTime = float.MaxValue;
                return;

            case FlickeringFrequency.AlmostUnnoticeable:
                offDuration = Random.Range(0.05f, 0.1f);
                onDuration = Random.Range(3f, 6f);
                break;

            case FlickeringFrequency.Annoying:
                offDuration = Random.Range(0.08f, 0.2f);
                onDuration = Random.Range(0.5f, 4f);
                break;

            case FlickeringFrequency.Heavy:
                offDuration = Random.Range(0.1f, 0.3f);
                onDuration = Random.Range(0.2f, 2f);
                break;

            case FlickeringFrequency.Nightmare:
                offDuration = Random.Range(0.1f, 1.5f);
                onDuration = Random.Range(0.05f, 0.5f);
                break;
        }

        elevatorLightsContainer.SetActive(false);
        OnLightFlicker?.Invoke(false);

        if (flickeringSounds.Count > 0)
        {
            flickeringSounds[Random.Range(0, flickeringSounds.Count)].Play();
        }

        Invoke(nameof(RestoreLights), offDuration);
        nextFlickerTime = Time.time + offDuration + onDuration;
    }

    private void RestoreLights()
    {
        if (elevatorLightsContainer != null)
        {
            elevatorLightsContainer.SetActive(true);
            OnLightFlicker?.Invoke(true);
        }
    }

    private void ApplyElevatorShaking()
    {
        if (!isMoving) return;

        switch (shakingLevel)
        {
            case ShakingIntensity.NoShaking:
                nextShakeTime = float.MaxValue;
                return;

            case ShakingIntensity.AlmostUnnoticeable:
                currentShakeMagnitude = Random.Range(0.05f, 0.1f);
                nextShakeTime = Time.time + Random.Range(2f, 6f);
                break;

            case ShakingIntensity.Annoying:
                currentShakeMagnitude = Random.Range(0.08f, 0.2f);
                nextShakeTime = Time.time + Random.Range(0.5f, 4f);
                break;

            case ShakingIntensity.Heavy:
                currentShakeMagnitude = Random.Range(0.1f, 0.3f);
                nextShakeTime = Time.time + Random.Range(0.2f, 2f);
                break;

            case ShakingIntensity.Nightmare:
                currentShakeMagnitude = Random.Range(0.1f, 1f);
                nextShakeTime = Time.time + Random.Range(0.5f, 1f);
                break;
        }

        OnShaking?.Invoke(currentShakeMagnitude);
    }

    private void ApplyShakeRotation()
    {
        elevatorGameObject.transform.rotation = originalElevatorRotation * Quaternion.Euler(
            Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
            Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
            Random.Range(-currentShakeMagnitude, currentShakeMagnitude)
        );
    }

    public IElevatorDoor[] GetAllDoors()
    {
        List<IElevatorDoor> allDoors = new List<IElevatorDoor>();
        allDoors.AddRange(internalDoors);
        allDoors.AddRange(externalDoors);
        return allDoors.ToArray();
    }

    public IElevatorDoor[] GetInternalDoors()
    {
        return internalDoors.ToArray();
    }

    public IElevatorDoor[] GetExternalDoors()
    {
        return externalDoors.ToArray();
    }

    public void OnInternalDoorStateChanged()
    {
        if (config.EmergencyStopIfInternalDoorsOpen && isMoving)
        {
            foreach (ElevatorDoor door in internalDoors)
            {
                if (door.CurrentState != DoorState.FullyClosed)
                {
                    Stop();
                    return;
                }
            }
        }

        if (config.ResumeAfterInternalDoorsClosed && !isMoving && !hasReachedDestination)
        {
            bool allClosed = true;
            foreach (ElevatorDoor door in internalDoors)
            {
                if (door.CurrentState != DoorState.FullyClosed)
                {
                    allClosed = false;
                    break;
                }
            }

            if (allClosed)
            {
                MoveTo(currentDestination);
            }
        }
    }
}
