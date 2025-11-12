using Assets.Scripts.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class ItemHighlight : MonoBehaviour
{
    [Header("Configuración del Outline")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField, Range(0f, 10f)] private float outlineWidth = 2f;

    private Renderer targetRenderer;
    [SerializeField] private Material outlineMaskMaterial;
    [SerializeField] private Material outlineFillMaterial;
    private bool isHighlighted = false;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();

        //// Cargar materiales desde Resources (asegurate de tenerlos en Resources/Materials/)
        //outlineMaskMaterial = Instantiate(Resources.Load<Material>("Materials/OutlineMask"));
        //outlineFillMaterial = Instantiate(Resources.Load<Material>("Materials/OutlineFill"));

        if (outlineMaskMaterial == null || outlineFillMaterial == null)
        {
            Debug.LogError($"[{nameof(ItemHighlight)}] No se encontraron los materiales Outline en Resources/Materials/");
        }
    }

    public void EnableHighlight()
    {
        if (isHighlighted || targetRenderer == null) return;
        UIManager.Instance.ShowInteractionPanel();
        isHighlighted = true;

        // Copiar materiales actuales
        var mats = new List<Material>(targetRenderer.sharedMaterials)
        {
            // Añadir outline
            outlineMaskMaterial,
            outlineFillMaterial
        };

        targetRenderer.materials = mats.ToArray();

        UpdateOutlineProperties();
    }

    public void DisableHighlight()
    {
        if (!isHighlighted || targetRenderer == null) return;
        UIManager.Instance.HidePanel(UIPanelTypeEnum.Interaction);

        isHighlighted = false;

        var mats = new List<Material>(targetRenderer.sharedMaterials);
        mats.Remove(outlineMaskMaterial);
        mats.Remove(outlineFillMaterial);

        targetRenderer.materials = mats.ToArray();
    }

    private void UpdateOutlineProperties()
    {
        if (outlineFillMaterial != null)
        {
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
            outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }
    }

    void OnDestroy()
    {
        if (outlineMaskMaterial != null) targetRenderer.materials.ToList().Remove(outlineMaskMaterial);
        if (outlineFillMaterial != null) targetRenderer.materials.ToList().Remove(outlineFillMaterial);
    }
}