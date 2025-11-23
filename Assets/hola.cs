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
    }
}
