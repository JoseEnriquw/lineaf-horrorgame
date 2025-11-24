using UnityEngine;
using System.Collections;
using UHFPS.Runtime;

[RequireComponent(typeof(AudioSource))]
public class AudioPistaPuzzle : MonoBehaviour
{
    [Header("Configuración del Audio")]
    [Tooltip("El clip de audio con la voz del anuncio")]
    [SerializeField] private AudioClip anuncioClip;

    [Header("Intervalos de Tiempo (Segundos)")]
    [Tooltip("Tiempo mínimo de espera entre reproducciones")]
    [SerializeField] private float tiempoMinimo = 10f;
    [Tooltip("Tiempo máximo de espera (para variar el ritmo)")]
    [SerializeField] private float tiempoMaximo = 20f;

    [Header("Estado")]
    [Tooltip("Si es true, el audio dejará de sonar. Útil para debug.")]
    public bool puzzleResuelto = false;

    private AudioSource audioSource;
    private Coroutine rutinaAudio;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = anuncioClip;

        // Configuración para sonido 3D (importante para inmersión)
        audioSource.spatialBlend = 1.0f; // 1.0 es totalmente 3D
        audioSource.minDistance = 2f;    // Distancia donde se escucha full volumen
        audioSource.maxDistance = 15f;   // Distancia donde deja de escucharse

        // Iniciamos el bucle del anuncio
        rutinaAudio = StartCoroutine(ReproducirAnuncioFantasma());
    }

    // Corutina que maneja el bucle de reproducción
    IEnumerator ReproducirAnuncioFantasma()
    {
        // Esperamos un poco al iniciar para no asustar de golpe (opcional)
        yield return new WaitForSeconds(2f);

        while (!puzzleResuelto)
        {
            if (anuncioClip != null)
            {
                audioSource.Play();
                GameManager.Instance.ShowHintMessage("Atención pasajeros..... el servicio de la Línea F con destino a.......... partirá desde el andén 4..... a las 0: 2..... 5.",5f);
                // Ejemplo: UIManager.Instance.MostrarSubtitulo("Atención pasajeros... andén 4...");
            }

            // Calculamos cuánto dura el audio para no interrumpirlo
            float duracionAudio = anuncioClip != null ? anuncioClip.length : 0f;

            // Calculamos el tiempo de espera aleatorio para generar tensión
            float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);

            // Esperamos a que termine el audio + el tiempo de silencio
            yield return new WaitForSeconds(duracionAudio + tiempoEspera);
        }
    }

    // ---------------------------------------------------------
    // MÉTODO PÚBLICO: Llama a esto desde tu script del Candado/Puzzle
    // ---------------------------------------------------------
    public void ResolverPuzzle()
    {
        if (puzzleResuelto) return;

        puzzleResuelto = true;

        // Opción A: Detener el audio inmediatamente
        audioSource.Stop();

        // Opción B: Destruir este componente o el objeto para ahorrar recursos
        // Destroy(this); // Destruye solo el script
        // Destroy(gameObject, 0.5f); // Destruye el objeto (altavoz) tras 0.5s

        Debug.Log("Puzzle resuelto: Audio pista detenido.");
    }
}