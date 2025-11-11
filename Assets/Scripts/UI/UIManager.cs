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
        [SerializeField] private TextMeshProUGUI txtInventoryTitle;
        [SerializeField] private GridLayoutGroup inventoryGrid;
        [SerializeField] private GameObject inventoryItemPrefab;

        // ===== PANEL DE INTERACCIÓN =====
        [Header("Panel de Interacción")]
        [SerializeField] private GameObject panelInteraction;
        [SerializeField] private TextMeshProUGUI txtInteractionTitle;
        [SerializeField] private TextMeshProUGUI txtInteractionDescription;
        [SerializeField] private Button buttonInteractionConfirm;
        [SerializeField] private Button buttonInteractionCancel;

        // ===== DICCIONARIO DE PANELES =====
        private Dictionary<UIPanelTypeEnum, GameObject> panels;

        // ===== CALLBACKS =====
        private System.Action onInteractionConfirm;
        private System.Action onInteractionCancel;

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
            SetupButtonListeners();
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

            if (panelInventory != null && inventoryGrid == null)
            {
                Debug.LogWarning("[GeneralUIManager] El GridLayoutGroup del inventario no está asignado.");
            }

            if (panelInteraction != null && (buttonInteractionConfirm == null || buttonInteractionCancel == null))
            {
                Debug.LogWarning("[GeneralUIManager] Los botones del panel de interacción no están asignados.");
            }
        }

        /// <summary>
        /// Configura los listeners de los botones
        /// </summary>
        private void SetupButtonListeners()
        {
            if (buttonInteractionConfirm != null)
            {
                buttonInteractionConfirm.onClick.AddListener(OnConfirmInteraction);
            }

            if (buttonInteractionCancel != null)
            {
                buttonInteractionCancel.onClick.AddListener(OnCancelInteraction);
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

                Debug.Log($"[GeneralUIManager] Panel ocultado: {typePanel}", gameObject);
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
        public void ShowInventoryPanel(string title = "Inventario")
        {
            ShowPanel(UIPanelTypeEnum.Inventory);

            if (txtInventoryTitle != null)
            {
                txtInventoryTitle.text = title;
            }
        }

        /// <summary>
        /// Agrega un item visual al inventario
        /// </summary>
        public void AddInventoryItem(string itemName, Sprite itemIcon)
        {
            if (inventoryGrid == null)
            {
                Debug.LogError("[GeneralUIManager] El GridLayoutGroup del inventario no está asignado.", gameObject);
                return;
            }

            GameObject itemGO = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
            Image itemImage = itemGO.GetComponent<Image>();

            if (itemImage != null)
            {
                itemImage.sprite = itemIcon;
            }

            TextMeshProUGUI itemLabel = itemGO.GetComponentInChildren<TextMeshProUGUI>();
            if (itemLabel != null)
            {
                itemLabel.text = itemName;
            }

            Debug.Log($"[GeneralUIManager] Item agregado al inventario: {itemName}", gameObject);
        }

        /// <summary>
        /// Limpia todos los items del inventario
        /// </summary>
        public void ClearInventory()
        {
            if (inventoryGrid == null)
                return;

            foreach (Transform child in inventoryGrid.transform)
            {
                Destroy(child.gameObject);
            }

            Debug.Log("[GeneralUIManager] Inventario limpiado.", gameObject);
        }

        // ===== MÉTODOS DEL PANEL DE INTERACCIÓN =====

        /// <summary>
        /// Muestra el panel de interacción con título y descripción
        /// Permite asignar callbacks para los botones
        /// </summary>
        public void ShowInteractionPanel(string title, string description, 
            System.Action onConfirm = null, System.Action onCancel = null)
        {
            ShowPanel(UIPanelTypeEnum.Interaction);

            if (txtInteractionTitle != null)
            {
                txtInteractionTitle.text = title;
            }

            if (txtInteractionDescription != null)
            {
                txtInteractionDescription.text = description;
            }

            // Asignar callbacks
            onInteractionConfirm = onConfirm;
            onInteractionCancel = onCancel;

            Debug.Log($"[GeneralUIManager] Panel de interacción mostrado: {title}", gameObject);
        }

        /// <summary>
        /// Se ejecuta cuando el usuario confirma la interacción
        /// </summary>
        private void OnConfirmInteraction()
        {
            Debug.Log("[GeneralUIManager] Interacción confirmada.", gameObject);

            onInteractionConfirm?.Invoke();
            HideAllPanels();
        }

        /// <summary>
        /// Se ejecuta cuando el usuario cancela la interacción
        /// </summary>
        private void OnCancelInteraction()
        {
            Debug.Log("[GeneralUIManager] Interacción cancelada.", gameObject);

            onInteractionCancel?.Invoke();
            HideAllPanels();
        }

        /// <summary>
        /// Método alternativo para mostrar interacción sin callbacks
        /// (Útil si solo necesitas mostrar información)
        /// </summary>
        public void ShowInteractionPanelSimple(string title, string description)
        {
            ShowInteractionPanel(title, description);
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
