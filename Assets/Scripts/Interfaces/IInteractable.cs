public interface IInteractable
{
    void OnLookAt();
    void OnLookAway();
    void OnInteract();
}

public interface IInteractableItems: IInteractable
{
    InventoryItem GetItemData();
}