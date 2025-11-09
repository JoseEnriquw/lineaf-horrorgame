using UnityEngine;

/// <summary>
/// Configuración para el movimiento del elevador.
/// Encapsula todos los parámetros de movimiento en un ScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "ElevatorConfig", menuName = "Elevator/Configuration")]
public class ElevatorConfiguration : ScriptableObject
{
    [Header("Movimento")]
    [SerializeField] private float unitsPerSecond = 1.5f;
    [SerializeField] private bool slowDownNearDestination = true;
    [SerializeField] private float slowingDownEffect = 0.02f;

    [Header("Seguridad")]
    [SerializeField] private bool internalDoorsMustBeClosed = true;
    [SerializeField] private bool externalDoorsMustBeClosed = true;
    [SerializeField] private bool emergencyStopIfInternalDoorsOpen = true;
    [SerializeField] private bool resumeAfterInternalDoorsClosed = true;

    public float UnitsPerSecond => unitsPerSecond;
    public bool SlowDownNearDestination => slowDownNearDestination;
    public float SlowingDownEffect => slowingDownEffect;
    public bool InternalDoorsMustBeClosed => internalDoorsMustBeClosed;
    public bool ExternalDoorsMustBeClosed => externalDoorsMustBeClosed;
    public bool EmergencyStopIfInternalDoorsOpen => emergencyStopIfInternalDoorsOpen;
    public bool ResumeAfterInternalDoorsClosed => resumeAfterInternalDoorsClosed;
}
