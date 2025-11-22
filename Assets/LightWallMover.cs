using UnityEngine;

public class LightWallMover : MonoBehaviour
{
    public float speed = 10f;
    private bool shouldMove = false;

    public void Activate()
    {
        shouldMove = true;
    }

    void Update()
    {
        if (shouldMove)
            transform.Translate(Vector3.left * speed * Time.deltaTime); 
    }
}
