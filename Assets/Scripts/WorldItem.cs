using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractableItems
{
    [SerializeField] private InventoryItem itemData;

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip pickupSound;

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

    public InventoryItem GetItemData() => itemData;

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

    public void OnInteract()
    {
        // Reproducir sonido
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Destruir el objeto
        Destroy(gameObject);
    }
}