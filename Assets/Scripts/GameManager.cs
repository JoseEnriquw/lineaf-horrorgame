using System;
using UnityEngine;

/// <summary>
/// GameManager singleton para manejar estados globales del juego.
/// Similar a UIManager, persiste entre escenas y expone un evento
/// para bloquear/desbloquear paneles.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Evento que notifica el cambio de estado de bloqueo de paneles.
    public Action<bool> OnChangePlayerInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetEnablePlayerInput(bool enable)
    {
        OnChangePlayerInput?.Invoke(enable);
        if (enable) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
