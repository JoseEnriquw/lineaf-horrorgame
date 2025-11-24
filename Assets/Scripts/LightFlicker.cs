using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Componentes")]
    [Tooltip("Arrastra aquí las luces (Point/Spot Light) que iluminan el entorno")]
    [SerializeField] Light[] luces;

    [Tooltip("Arrastra aquí el objeto 3D (MeshRenderer) del tubo o foco. Se creará una instancia de su material automáticamente.")]
    [SerializeField] Renderer[] objetosEmisivos;

    [Header("Tiempos")]
    [SerializeField] float tiempoMinimo = 0.1f;
    [SerializeField] float tiempoMaximo = 1f;

    // Variables internas
    private float timer = 0f;
    private float tiempoEncendido = 0f;
    private float tiempoApagado = 0f;
    private bool estaEncendido = true;

    // Guardamos el color original de la emisión para no perderlo
    private Color[] coloresEmisionOriginales;
    // Guardamos las referencias a las COPIAS de los materiales
    private Material[] materialesInstanciados;

    private void Start()
    {
        // Si olvidaste asignar las luces, intenta buscarlas en este objeto
        if (luces == null || luces.Length == 0)
            luces = GetComponentsInChildren<Light>();

        InicializarMateriales();
        AsignarTiemposAleatorios();
    }

    private void InicializarMateriales()
    {
        if (objetosEmisivos != null && objetosEmisivos.Length > 0)
        {
            materialesInstanciados = new Material[objetosEmisivos.Length];
            coloresEmisionOriginales = new Color[objetosEmisivos.Length];

            for (int i = 0; i < objetosEmisivos.Length; i++)
            {
                if (objetosEmisivos[i] != null)
                {
                    // ALERTA DE MAGIA DE UNITY:
                    // Al usar .material (y no .sharedMaterial), Unity crea una COPIA de ese material
                    // solo para este objeto. Así no rompes los demás objetos de la escena.
                    materialesInstanciados[i] = objetosEmisivos[i].material;

                    // Habilitamos la palabra clave de emisión por si acaso no estaba activa
                    materialesInstanciados[i].EnableKeyword("_EMISSION");

                    // Guardamos el color de emisión que le pusiste en el editor (ej: blanco fuerte, amarillo, etc.)
                    if (materialesInstanciados[i].HasProperty("_EmissionColor"))
                    {
                        coloresEmisionOriginales[i] = materialesInstanciados[i].GetColor("_EmissionColor");
                    }
                }
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (estaEncendido && timer >= tiempoEncendido)
        {
            CambiarEstado(false); // Apagar
            estaEncendido = false;
            timer = 0f;
            AsignarTiemposAleatorios();
        }
        else if (!estaEncendido && timer >= tiempoApagado)
        {
            CambiarEstado(true); // Encender
            estaEncendido = true;
            timer = 0f;
            AsignarTiemposAleatorios();
        }
    }

    private void CambiarEstado(bool encender)
    {
        // 1. Controlar la Luz real (Point/Spot Light)
        foreach (Light l in luces)
        {
            if (l != null) l.enabled = encender;
        }

        // 2. Controlar la Emisión del Material (Lo visual)
        if (materialesInstanciados != null)
        {
            for (int i = 0; i < materialesInstanciados.Length; i++)
            {
                if (materialesInstanciados[i] != null)
                {
                    if (encender)
                    {
                        // Restauramos el color EXACTO que tenía al principio
                        materialesInstanciados[i].SetColor("_EmissionColor", coloresEmisionOriginales[i]);
                    }
                    else
                    {
                        // Ponemos la emisión en NEGRO (que visualmente es apagado)
                        // Esto no cambia el color del objeto (Albedo), solo le quita el brillo.
                        materialesInstanciados[i].SetColor("_EmissionColor", Color.black);
                    }

                    // Asegura que el sistema de iluminación global se entere del cambio
                    DynamicGI.SetEmissive(objetosEmisivos[i], encender ? coloresEmisionOriginales[i] : Color.black);
                }
            }
        }
    }

    private void AsignarTiemposAleatorios()
    {
        tiempoEncendido = Random.Range(tiempoMinimo, tiempoMaximo);
        tiempoApagado = Random.Range(tiempoMinimo, tiempoMaximo);
    }
}