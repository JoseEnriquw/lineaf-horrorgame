using UnityEngine;

public class RandomBlink : MonoBehaviour
{
    [SerializeField] private Light targetLight; // Luz a controlar
    [SerializeField] private float minInterval = 0.1f; // Tiempo mínimo entre parpadeos
    [SerializeField] private float maxInterval = 1.5f; // Tiempo máximo entre parpadeos
    [SerializeField] private float minOffTime = 0.05f; // Cuánto tiempo puede estar apagada
    [SerializeField] private float maxOffTime = 0.3f;  // Cuánto tiempo puede estar apagada

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        StartCoroutine(BlinkLoop());
    }

    private System.Collections.IEnumerator BlinkLoop()
    {
        while (true)
        {
            // Espera un tiempo random antes del próximo parpadeo
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            // Apaga la luz
            targetLight.enabled = false;

            // Mantiene la luz apagada un tiempo random
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));

            // Vuelve a encenderla
            targetLight.enabled = true;
        }
    }
}
