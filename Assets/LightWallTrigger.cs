using UnityEngine;

public class LightWallTrigger : MonoBehaviour
{
    public LightWallMover lightWall;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lightWall.Activate();
        }
    }
}