using UnityEngine;
using UHFPS.Runtime;

public class VictoryTrigger : MonoBehaviour
{
    [Tooltip("Música que sonará al ganar. Se asignará al AudioSource configurado en el VictoryModule.")]
    public AudioClip VictoryMusic;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador
        if (other.CompareTag("Player"))
        {
            // Llamamos a nuestro módulo de victoria pasando el clip de música
            GameManager.Module<VictoryModule>().TriggerVictory(VictoryMusic);

            // Desactivamos este trigger para que no salte dos veces
            gameObject.SetActive(false);
        }
    }
}