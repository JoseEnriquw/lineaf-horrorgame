using UnityEngine;

public class LightWallMover : MonoBehaviour
{

    public Transform player; 
    public float speed = 10f;
    private bool shouldMove = false;

    public void Activate()
    {
        shouldMove = true;
    }

    void Update()
    {
        if (shouldMove)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }
}
