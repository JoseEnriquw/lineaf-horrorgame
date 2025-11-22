using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathInSubte : MonoBehaviour
{
    [SerializeField] private string sceneName;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
