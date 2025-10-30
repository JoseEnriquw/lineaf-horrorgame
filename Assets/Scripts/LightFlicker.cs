using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Light[] luces;            // Array de luces a controlar
    [SerializeField] float tiempoMinimo = 0.1f;  // Tiempo mínimo de encendido/apagado
    [SerializeField] float tiempoMaximo = 1f;    // Tiempo máximo de encendido/apagado

    // Variables internas
    private float timer = 0f;
    private float tiempoEncendido = 0f;
    private float tiempoApagado = 0f;
    private bool lucesEncendidas = true;

    private void Start()
    {
        // Si no se asignaron luces, se intentan obtener todas las del objeto
        if (luces == null || luces.Length == 0)
        {
            luces = GetComponentsInChildren<Light>();
        }

        AsignarTiemposAleatorios();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (lucesEncendidas && timer >= tiempoEncendido)
        {
            CambiarEstadoLuces(false);
            lucesEncendidas = false;
            timer = 0f;
            AsignarTiemposAleatorios();
        }
        else if (!lucesEncendidas && timer >= tiempoApagado)
        {
            CambiarEstadoLuces(true);
            lucesEncendidas = true;
            timer = 0f;
            AsignarTiemposAleatorios();
        }
    }

    private void CambiarEstadoLuces(bool encender)
    {
        foreach (Light l in luces)
        {
            if (l != null)
                l.enabled = encender;
        }
    }

    private void AsignarTiemposAleatorios()
    {
        tiempoEncendido = Random.Range(tiempoMinimo, tiempoMaximo);
        tiempoApagado = Random.Range(tiempoMinimo, tiempoMaximo);
    }
}
