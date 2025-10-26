using UnityEngine;
using System.Collections.Generic;

// Esto crea una opción en el menú de Unity
// para crear este asset: Assets > Create > LED > Pixel Font
[CreateAssetMenu(fileName = "NewPixelFont", menuName = "LED/Pixel Font Asset")]
public class PixelFontAsset : ScriptableObject
{
    public int charWidth = 5;
    public int charHeight = 7;
    public int spacing = 1;

    // Aquí definiremos todos los caracteres
    public List<CharacterGlyph> glyphs;

    // Pequeña ayuda para validar los datos desde el inspector
    void OnValidate()
    {
        int expectedSize = charWidth * charHeight;
        foreach (var glyph in glyphs)
        {
            if (glyph.glyphData == null || glyph.glyphData.Length != expectedSize)
            {
                // Si el tamaño no coincide, lo reinicia
                glyph.glyphData = new bool[expectedSize];
                Debug.LogWarning($"Glyph '{glyph.character}' in {this.name} was resized to {expectedSize} elements.");
            }
        }
    }
}