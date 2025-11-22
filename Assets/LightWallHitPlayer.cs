using UnityEngine;

public class LightWallHitPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public FlashToWhite flash;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flash.TriggerFlash();
        }
    }
}
