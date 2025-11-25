using UnityEngine;

public class AbrirPuertasSubte : MonoBehaviour
{
    [SerializeField] private Animator puertas;

    public void AbrirPuertas()
    {
        puertas.Play("abrirPuertas");
    } 
}
