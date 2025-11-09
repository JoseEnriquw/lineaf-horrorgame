using UnityEngine;

/// <summary>
/// Interfaz para definir el comportamiento de un elevador.
/// Desacopla la lógica del elevador de su implementación específica.
/// </summary>
public interface IElevator
{
    /// <summary>
    /// Posición actual del elevador
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Determina si el elevador está en movimiento
    /// </summary>
    bool IsMoving { get; }

    /// <summary>
    /// Mueve el elevador a una posición específica
    /// </summary>
    void MoveTo(Vector3 targetPosition);

    /// <summary>
    /// Detiene el elevador
    /// </summary>
    void Stop();

    /// <summary>
    /// Obtiene todas las puertas del elevador
    /// </summary>
    IElevatorDoor[] GetAllDoors();

    /// <summary>
    /// Obtiene las puertas internas (dentro del elevador)
    /// </summary>
    IElevatorDoor[] GetInternalDoors();

    /// <summary>
    /// Obtiene las puertas externas (en los pisos)
    /// </summary>
    IElevatorDoor[] GetExternalDoors();
}
