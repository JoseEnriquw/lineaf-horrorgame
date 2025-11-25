using UnityEngine;
using UnityEngine.SceneManagement;

public class FlashToWhite : MonoBehaviour
{
    public CanvasGroup cg;
    public float fadeSpeed = 1.5f;
    private bool triggered = false;
    public string nextSceneName;

    public void TriggerFlash()
    {
        triggered = true;
    }

    void Update()
    {
        if (triggered)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 1f, Time.deltaTime * fadeSpeed);

            if (cg.alpha > 0.98f)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
