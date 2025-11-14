using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// UIManager general para gestionar paneles de UI del juego
    /// Implementa el patrón Singleton
    /// Gestiona: Panel de Inventario y Panel de Interacción
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance => instance;

        private UIPanelTypeEnum? currentPanel = null;

        // ===== PANEL DE INVENTARIO =====
        [Header("Panel de Inventario")]
        [SerializeField] private GameObject panelInventory;

        // ===== PANEL DE INTERACCIÓN =====
        [Header("Panel de Interacción")]
        [SerializeField] private GameObject panelInteraction;
        // ===== DICCIONARIO DE PANELES =====
        private Dictionary<UIPanelTypeEnum, GameObject> panels;

        private void Awake()
        {
            // Singleton Pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializePanels();
            HideAllPanels();
        }

        /// <summary>
        /// Inicializa el diccionario de paneles
        /// </summary>
        private void InitializePanels()
        {
            panels = new Dictionary<UIPanelTypeEnum, GameObject>
            {
                { UIPanelTypeEnum.Inventory, panelInventory },
                { UIPanelTypeEnum.Interaction, panelInteraction },
            };

            // Validar que todos los paneles estén asignados
            ValidatePanels();
        }

        /// <summary>
        /// Valida que todos los paneles y componentes estén correctamente asignados
        /// </summary>
        private void ValidatePanels()
        {
            foreach (var panelEntry in panels)
            {
                if (panelEntry.Value == null)
                {
                    Debug.LogError($"[GeneralUIManager] El panel {panelEntry.Key} no está asignado en el Inspector.");
                }
            }
        }

        /// <summary>
        /// Muestra un panel específico
        /// </summary>
        public void ShowPanel(UIPanelTypeEnum typePanel)
        {
            // Si el panel actual es el mismo, ignorar
            if (currentPanel == typePanel)
                return;

            // Ocultar panel actual si existe
            if (currentPanel.HasValue && panels.TryGetValue(currentPanel.Value, out GameObject currentPanelGO))
            {
                currentPanelGO.SetActive(false);
            }

            // Mostrar nuevo panel
            if (panels.TryGetValue(typePanel, out GameObject panelToShow))
            {
                panelToShow.SetActive(true);
                currentPanel = typePanel;

                Debug.Log($"[GeneralUIManager] Panel mostrado: {typePanel}", gameObject);
            }
            else
            {
                Debug.LogError($"[GeneralUIManager] El panel {typePanel} no se encontró en el diccionario.", gameObject);
            }
        }

        /// <summary>
        /// Oculta un panel específico
        /// </summary>
        public void HidePanel(UIPanelTypeEnum typePanel)
        {
            if (panels.TryGetValue(typePanel, out GameObject panelToHide))
            {
                panelToHide.SetActive(false);

                // Si es el panel actual, limpiar referencia
                if (currentPanel == typePanel)
                {
                    currentPanel = null;
                }

                //Debug.Log($"[GeneralUIManager] Panel ocultado: {typePanel}", gameObject);
            }
            else
            {
                Debug.LogError($"[GeneralUIManager] El panel {typePanel} no se encontró en el diccionario.", gameObject);
            }
        }

        /// <summary>
        /// Oculta todos los paneles
        /// </summary>
        public void HideAllPanels()
        {
            Cursor.lockState = CursorLockMode.Locked;
            foreach (var panelEntry in panels)
            {
                panelEntry.Value.SetActive(false);
            }
            currentPanel = null;

            Debug.Log("[GeneralUIManager] Todos los paneles ocultados.", gameObject);
        }

        /// <summary>
        /// Retorna el panel actualmente visible
        /// </summary>
        public UIPanelTypeEnum? GetCurrentPanel()
        {
            return currentPanel;
        }

        // ===== MÉTODOS DEL PANEL DE INVENTARIO =====

        /// <summary>
        /// Muestra el panel de inventario y actualiza su contenido
        /// </summary>
        public void ShowInventoryPanel()
        {
            GameManager.Instance.SetEnablePlayerInput(false);
            Cursor.lockState = CursorLockMode.Confined;
            ShowPanel(UIPanelTypeEnum.Inventory);
        }

        // ===== MÉTODOS DEL PANEL DE INTERACCIÓN =====

        /// <summary>
        /// Muestra el panel de interacción con título y descripción
        /// Permite asignar callbacks para los botones
        /// </summary>
        public void ShowInteractionPanel()
        {
            ShowPanel(UIPanelTypeEnum.Interaction);
        }

        // ===== UTILIDADES =====

        /// <summary>
        /// Retorna si hay algún panel visible
        /// </summary>
        public bool IsAnyPanelVisible()
        {
            return currentPanel.HasValue;
        }

        /// <summary>
        /// Cierra los paneles si presionas ESC
        /// (Opcional, puedes mover a un InputManager)
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && IsAnyPanelVisible())
            {
                HideAllPanels();
            }
        }
    }
}
