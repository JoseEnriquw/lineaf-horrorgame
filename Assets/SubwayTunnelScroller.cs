using UnityEngine;

public class SubwayTunnelScroller : MonoBehaviour
{
    public float speed = 10f;      // Velocidad aparente del tren
    private Vector3 startPos;
    private float repeatLength;    // Largo del módulo del túnel

    void Start()
    {
        startPos = transform.position;

        // Tomar el largo del túnel en el eje X
        BoxCollider box = GetComponent<BoxCollider>();
        repeatLength = box.size.x * transform.localScale.x;
    }

    void Update()
    {
        // Mover el túnel hacia la izquierda (X negativo)
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // Cuando el túnel se desplazó suficientemente hacia atrás, lo reiniciamos adelante
        if (transform.position.x < startPos.x - repeatLength)
        {
            transform.position += Vector3.right * repeatLength;
            startPos = transform.position;
        }
    }
}
