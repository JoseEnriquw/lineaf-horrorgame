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
        public void TriggerVictory()
        {
            // Buscamos la referencia "Victory" en el GameManager
            if (GameManager.GraphicReferences.Value.TryGetValue("Victory", out Behaviour[] uiRefs))
            {
                // Esperamos el siguiente orden en las referencias:
                // [0] -> CanvasGroup (El Panel)
                // [1] -> Button (Botón Volver al Menú) - Opcional
                // [2] -> AudioSource (Música de Fondo) - Opcional

                if (uiRefs.Length > 0 && uiRefs[0] is CanvasGroup panel)
                {
                    UnityEngine.UI.Button menuButton = null;
                    AudioSource victoryMusic = null;

                    // Intentamos obtener el botón si existe
                    if (uiRefs.Length > 1 && uiRefs[1] is UnityEngine.UI.Button btn) 
                        menuButton = btn;

                    // Intentamos obtener el audio si existe
                    if (uiRefs.Length > 2 && uiRefs[2] is AudioSource audio) 
                        victoryMusic = audio;

                    RunCoroutine(ShowVictoryRoutine(panel, menuButton, victoryMusic));
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

        private IEnumerator ShowVictoryRoutine(CanvasGroup panel, UnityEngine.UI.Button menuBtn, AudioSource music)
        {
            // 0. Asegurar que el objeto esté activo
            panel.gameObject.SetActive(true);
            panel.alpha = 0f; // Asegurar que empiece invisible para el fade

            // 1. Configurar el estado del juego (Congelar, ocultar HUD)
            GameManager.FreezePlayer(true, showCursor: true, lockInput: true);
            GameManager.DisableAllGamePanels();
            GameManager.OverlaysParent.SetActive(false);

            // 2. Configurar el botón de menú
            if (menuBtn != null)
            {
                menuBtn.onClick.RemoveAllListeners();
                menuBtn.onClick.AddListener(() => 
                {
                    // Usamos la función nativa de UHFPS para volver al menú
                    GameManager.MainMenu();
                });
            }

            // 3. Reproducir música
            if (music != null)
            {
                music.Play();
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
