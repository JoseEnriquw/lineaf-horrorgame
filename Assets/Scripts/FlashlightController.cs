using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Configuración de la Linterna")]
    [SerializeField] private string flashlightID = "flashlight"; // Debe coincidir con el itemID
    [SerializeField] private Light flashlight;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip toggleSound;

    private bool isOn = false;
    private bool isEquipped = false;

    void OnEnable()
    {
        // Suscribirse a eventos
        InventoryEvents.OnItemEquipped.AddListener(OnItemEquipped);
        InventoryEvents.OnItemUnequipped.AddListener(OnItemUnequipped);
    }

    void OnDisable()
    {
        // Desuscribirse de eventos
        InventoryEvents.OnItemEquipped.RemoveListener(OnItemEquipped);
        InventoryEvents.OnItemUnequipped.RemoveListener(OnItemUnequipped);
    }

    void Start()
    {
        // Buscar componente Light si no está asignado
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
        }

        // Iniciar apagada
        if (flashlight != null)
        {
            flashlight.enabled = false;
        }
    }

    void Update()
    {
        // Solo permitir toggle si está equipada
        if (isEquipped && Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    private void OnItemEquipped(string itemID)
    {
        // Verificar si es nuestra linterna
        if (itemID == flashlightID)
        {
            isEquipped = true;
            Debug.Log("Linterna equipada - Presiona F para encender/apagar");
        }
    }

    private void OnItemUnequipped(string itemID)
    {
        // Verificar si es nuestra linterna
        if (itemID == flashlightID)
        {
            isEquipped = false;

            // Apagar la luz
            if (isOn)
            {
                isOn = false;
                if (flashlight != null)
                {
                    flashlight.enabled = false;
                }
            }

            Debug.Log("Linterna desequipada");
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlight == null) return;

        isOn = !isOn;
        flashlight.enabled = isOn;

        // Reproducir sonido
        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }

        Debug.Log($"Linterna {(isOn ? "encendida" : "apagada")}");
    }

    // Métodos públicos
    public bool IsEquipped() => isEquipped;
    public bool IsOn() => isOn;
}