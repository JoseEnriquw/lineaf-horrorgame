using UnityEngine;
using UHFPS.Runtime;

public class VictoryTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador
        if (other.CompareTag("Player"))
        {
            // Llamamos a nuestro módulo de victoria
            GameManager.Module<VictoryModule>().TriggerVictory();

            // Desactivamos este trigger para que no salte dos veces
            gameObject.SetActive(false);
        }
    }
}
