using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Script simple para cada slot del inventario
/// Al pasar mouse muestra descripción
/// Al clickear equipa el item
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image itemIcon;

    private InventoryItem currentItem;
    private int slotIndex = -1;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSlotClicked);
        }
    }

    /// <summary>
    /// Al pasar mouse, muestra descripción
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && descriptionText != null)
        {
            descriptionText.text = currentItem.description;
            descriptionText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Al quitar mouse, oculta descripción
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Al clickear, equipa el item
    /// </summary>
    private void OnSlotClicked()
    {
        if (currentItem != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.EquipItemAtSlot(slotIndex);
        }
    }

    /// <summary>
    /// Asigna el item a este slot
    /// </summary>
    public void SetItem(InventoryItem item, int index)
    {
        currentItem = item;
        slotIndex = index;
        if (itemIcon != null)
        {
            itemIcon.enabled = item != null;
            itemIcon.sprite = item != null ? item.icon : null;
        }
    }

    public InventoryItem GetItem() => currentItem;
}
