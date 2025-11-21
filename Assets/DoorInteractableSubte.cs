using UnityEngine;

public class DoorInteractableSubte : MonoBehaviour, IInteractable
{

    public float interactRange = 2.0f; // distancia de activación
    public KeyCode interactKey = KeyCode.E;
    public Transform player; // asignar en inspector (o buscar por tag)
    public bool startsOpen = false;
    public AudioClip openSfx;
    public AudioClip closeSfx;
    public bool autoClose = true;
    public float autoCloseDelay = 3f;

    Animator animator;
    AudioSource audioSource;
    bool isOpen = false;
    float lastOpenedTime = -999f;

    private ItemHighlight highlight;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isOpen = startsOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleDoor()
    {
        SetOpen(!isOpen);
    }

    public void SetOpen(bool open)
    {
        if (isOpen == open) return;
        isOpen = open;
        animator.SetBool("IsOpen", isOpen);
        lastOpenedTime = Time.time;

        if (audioSource != null)
        {
            if (isOpen && openSfx != null) audioSource.PlayOneShot(openSfx);
            else if (!isOpen && closeSfx != null) audioSource.PlayOneShot(closeSfx);
        }
    }
    public void OnInteract()
    {
        ToggleDoor();
    }

    public void OnLookAt()
    {
        throw new System.NotImplementedException();
    }

    public void OnLookAway()
    {
        throw new System.NotImplementedException();
    }
}
