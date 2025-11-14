using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        // Esc para pausar o reanudar
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                //GameManager.Instance.SetEnablePlayerInput(true);
                //Cursor.lockState = CursorLockMode.Locked;
                Resume();
            }
            else
            {
                //GameManager.Instance.SetEnablePlayerInput(false);
                //Cursor.lockState = CursorLockMode.Confined;
                Pause();
            }

            isPaused = !isPaused;
        }

    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // congela el tiempo
        GameManager.Instance.SetEnablePlayerInput(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        GameManager.Instance.SetPaused(true);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameManager.Instance.SetEnablePlayerInput(true);
        Time.timeScale = 1f; // reanuda el tiempo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        GameManager.Instance.SetPaused(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu (Desktop)"); // cambia el nombre si tenés otra escena
    }
}
