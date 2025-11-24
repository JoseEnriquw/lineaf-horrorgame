using UnityEngine;


using UHFPS.Runtime;
public class hola : MonoBehaviour, IInteractStart
{
    public void InteractStart() {
        Hola();
    }
    public void Hola()
    {
        Debug.Log("holaaa");

        // Busca todos los scripts DoorOpen activos en la escena
        doorOpen[] doors = FindObjectsByType<doorOpen>(FindObjectsSortMode.None);

        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }
}
