using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemEquippedEvent : UnityEvent<string> { }

[System.Serializable]
public class ItemUnequippedEvent : UnityEvent<string> { }

[System.Serializable]
public class ItemPickedUpEvent : UnityEvent<string> { }

public static class InventoryEvents
{
    public static ItemEquippedEvent OnItemEquipped = new ItemEquippedEvent();
    public static ItemUnequippedEvent OnItemUnequipped = new ItemUnequippedEvent();
    public static ItemPickedUpEvent OnItemPickedUp = new ItemPickedUpEvent();
}

