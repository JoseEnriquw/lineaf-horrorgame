public interface IInteractable
{
    void OnLookAt();
    void OnLookAway();
    void OnInteract();
    InventoryItem GetItemData();
}