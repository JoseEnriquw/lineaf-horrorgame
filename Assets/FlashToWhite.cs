using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using ThunderWire.Attributes;
using UHFPS.Runtime;

public class FlashToWhite : MonoBehaviour
{
    public CanvasGroup cg;
    public float fadeSpeed = 1.5f;
    private bool triggered = false;
    public string nextSceneName;
    public BackgroundFader BackgroundFader;
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
                //SceneManager.LoadScene(nextSceneName);
                NewGame();
            }
        }
    }
    public void NewGame()
    {
        

        SaveGameManager.ClearLoadType();
        StartCoroutine(LoadNewGame());
    }

    IEnumerator LoadNewGame()
    {
        yield return BackgroundFader.StartBackgroundFade(false);
        

        SaveGameManager.LoadSceneName = "Hall_Anden";
        SceneManager.LoadScene(SaveGameManager.LMS);
    }
}
