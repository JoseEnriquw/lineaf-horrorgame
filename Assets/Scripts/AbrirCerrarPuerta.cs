using UnityEngine;

public class AbrirCerrarPuerta : MonoBehaviour
{
    public Animator laPuerta;

    private void OnTriggerEnter(Collider other)
    {
        laPuerta.Play("abrir");
    }

    private void OnTriggerExit(Collider other)
    {
        laPuerta.Play("cerrar");
    }
}
