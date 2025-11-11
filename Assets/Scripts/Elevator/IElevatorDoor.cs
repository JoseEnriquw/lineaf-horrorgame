using UnityEngine;

/// <summary>
/// Interfaz para definir el comportamiento de una puerta de elevador.
/// Permite desacoplar la lógica de control de puertas de su implementación.
/// </summary>
public interface IElevatorDoor
{
    /// <summary>
    /// Estado actual de la puerta
    /// </summary>
    DoorState CurrentState { get; }

    /// <summary>
    /// Abre la puerta
    /// </summary>
    void Open();

    /// <summary>
    /// Cierra la puerta
    /// </summary>
    void Close();

    /// <summary>
    /// Alterna el estado de la puerta (abre si está cerrada, cierra si está abierta)
    /// </summary>
    void Toggle();

    /// <summary>
    /// Bloquea/desbloquea la puerta
    /// </summary>
    void SetLocked(bool locked);

    /// <summary>
    /// Obtiene si la puerta está bloqueada
    /// </summary>
    bool IsLocked { get; }
}

/// <summary>
/// Estados posibles de una puerta
/// </summary>
public enum DoorState
{
    FullyClosed,
    FullyOpen,
    Moving
}
