using UnityEngine;

public class doorOpen : MonoBehaviour
{
    public Animator doorAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }
    }
    public void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
    }
}
