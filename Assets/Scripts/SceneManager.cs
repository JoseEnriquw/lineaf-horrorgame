using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Inicia la carga asíncrona de la escena "Nivel2" de forma aditiva
        StartCoroutine(LoadSceneAsync("Hall_Principal"));
        StartCoroutine(LoadSceneAsync("Subway_train"));

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        // SceneMode.Additive añade la escena a las ya cargadas
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Espera hasta que la carga esté completa
        while (!asyncLoad.isDone)
        {
            // Puedes usar asyncLoad.progress para una barra de carga
            Debug.Log("Cargando escena: " + asyncLoad.progress * 100 + "%");
            yield return null;
        }

        Debug.Log("Escena " + sceneName + " cargada de forma aditiva.");
    }
}
