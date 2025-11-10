using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CargadorPartida : MonoBehaviour
{

    [SerializeField] private string sceneToLoad = "GameScene"; // Nombre de la escena del juego
    [SerializeField] private Button playButton;

    void Start()
    {
        // Si no se asignó el botón manualmente, intenta obtenerlo del objeto actual
        if (playButton == null)
            playButton = GetComponent<Button>();

        // Asocia el método al evento del botón
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    void OnPlayButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
