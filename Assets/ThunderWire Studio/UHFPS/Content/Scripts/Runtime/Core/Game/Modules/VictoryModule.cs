using System;
using System.Collections;
using UnityEngine;
using UHFPS.Tools;

namespace UHFPS.Runtime
{
    [Serializable]
    public class VictoryModule : ManagerModule
    {
        public override string Name => "Victory Panel";

        /// <summary>
        /// Método público para activar la victoria.
        /// </summary>
        /// <param name="victoryClip">Clip de música opcional para reproducir.</param>
        public void TriggerVictory(AudioClip victoryClip = null)
        {
            // Buscamos la referencia "Victory" en el GameManager
            if (GameManager.GraphicReferences.Value.TryGetValue("Victory", out Behaviour[] uiRefs))
            {
                // Esperamos el siguiente orden en las referencias:
                // [0] -> CanvasGroup (El Panel)
                // [1] -> Button (Botón Volver al Menú)
                // [2] -> AudioSource (Source de Ambientación/Música)

                if (uiRefs.Length > 0 && uiRefs[0] is CanvasGroup panel)
                {
                    UnityEngine.UI.Button menuButton = null;
                    AudioSource ambientSource = null;

                    // Intentamos obtener el botón si existe
                    if (uiRefs.Length > 1 && uiRefs[1] is UnityEngine.UI.Button btn) 
                        menuButton = btn;

                    // Intentamos obtener el audio source de ambientación
                    if (uiRefs.Length > 2 && uiRefs[2] is AudioSource audio) 
                        ambientSource = audio;

                    RunCoroutine(ShowVictoryRoutine(panel, menuButton, ambientSource, victoryClip));
                }
                else
                {
                    Debug.LogError("[VictoryModule] La referencia 'Victory' [0] no es un CanvasGroup.");
                }
            }
            else
            {
                Debug.LogError("[VictoryModule] No se encontró la referencia UI 'Victory' en Custom UI References.");
            }
        }

        private IEnumerator ShowVictoryRoutine(CanvasGroup panel, UnityEngine.UI.Button menuBtn, AudioSource audioSource, AudioClip musicClip)
        {
            // 0. Asegurar que el objeto esté activo
            panel.gameObject.SetActive(true);
            panel.alpha = 0f;

            // 1. Configurar el estado del juego
            GameManager.FreezePlayer(true, showCursor: true, lockInput: true);
            
            // IMPORTANTE: Desactivar la interacción del panel de juego para que no bloquee los clics
            if (GameManager.GamePanel != null)
            {
                GameManager.GamePanel.interactable = false;
                GameManager.GamePanel.blocksRaycasts = false;
            }

            GameManager.DisableAllGamePanels();
            GameManager.OverlaysParent.SetActive(false);

            // 2. Configurar el botón de menú
            if (menuBtn != null)
            {
                menuBtn.onClick.RemoveAllListeners();
                menuBtn.onClick.AddListener(() => 
                {
                    GameManager.MainMenu();
                });
            }

            // 3. Configurar y reproducir música en el source de ambientación
            if (audioSource != null && musicClip != null)
            {
                audioSource.Stop(); // Detener lo que esté sonando
                audioSource.clip = musicClip;
                audioSource.loop = true; // Opcional: si quieres que se repita
                audioSource.Play();
            }

            // 4. Aplicar Blur
            if (GameManager.EnableBlur)
                GameManager.InterpolateBlur(GameManager.BlurRadius, GameManager.BlurDuration);

            // 5. Mostrar el panel con animación suave
            yield return CanvasGroupFader.StartFade(panel, true, 1.5f);
            
            // 6. Habilitar interacción
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }
}
