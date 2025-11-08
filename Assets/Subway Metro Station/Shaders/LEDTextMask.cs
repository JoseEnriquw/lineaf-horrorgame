using UnityEngine;
using System.Collections.Generic;

public class LEDTextRuntime : MonoBehaviour
{
    [Header("Shader Material")]
    public Material ledMaterial;

    [Header("Configuración de Fuente")]
    public int charWidth = 5;
    public int charHeight = 5;
    public int scale = 3;

    [Header("Espaciado")]
    public int letterSpacing = 1;
    public int lineThickness = 2;

    [Header("Tamaño del Panel")]
    public int maxCharsPerLine = 16;
    public int textureHeight = 30;

    private Texture2D maskTexture;
    private Dictionary<char, int[,]> fontMap;
    [SerializeField]
    private string defaulMessage = "BUENOS DIAS";

    void Start()
    {
        fontMap = LEDPixelFont5x5.CreateFont();
        GenerarTexto(defaulMessage);
    }

    public void GenerarTexto(string mensaje)
    {
        int scaledCharWidth = charWidth * scale;
        int scaledCharHeight = charHeight * scale;
        int scaledSpacing = letterSpacing * scale;
        int totalWidth = mensaje.Length * (scaledCharWidth + scaledSpacing);

        if (totalWidth < 64) totalWidth = 64;

        maskTexture = new Texture2D(totalWidth, textureHeight, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Point;
        maskTexture.wrapMode = TextureWrapMode.Clamp;

        Color32[] pixels = new Color32[maskTexture.width * maskTexture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;

        int cursorX = 0;

        foreach (char c in mensaje.ToUpper())
        {
            if (!fontMap.ContainsKey(c))
            {
                cursorX += scaledCharWidth + scaledSpacing;
                continue;
            }

            int[,] glyph = fontMap[c];

            for (int y = 0; y < charHeight; y++)
            {
                for (int x = 0; x < charWidth; x++)
                {
                    if (glyph[y, x] == 1)
                    {
                        for (int sy = 0; sy < scale; sy++)
                        {
                            for (int sx = 0; sx < scale; sx++)
                            {
                                int px = cursorX + x * scale + sx;
                                int py = (textureHeight - 1 - (y * scale + sy));

                                if (px < maskTexture.width && py < maskTexture.height)
                                {
                                    bool drawPixel = true;
                                    if (lineThickness < scale)
                                    {
                                        int offset = (scale - lineThickness) / 2;
                                        if (sx < offset || sx >= offset + lineThickness ||
                                            sy < offset || sy >= offset + lineThickness)
                                        {
                                            drawPixel = false;
                                        }
                                    }

                                    if (drawPixel)
                                        pixels[py * maskTexture.width + px] = Color.white;
                                }
                            }
                        }
                    }
                }
            }

            cursorX += scaledCharWidth + scaledSpacing;
        }

        maskTexture.SetPixels32(pixels);
        maskTexture.Apply();

        ledMaterial.SetTexture("_MaskTex", maskTexture);

        Debug.Log($"Textura LED: {maskTexture.width}x{maskTexture.height}px | Scale: {scale}x | Spacing: {scaledSpacing}px");
    }
}

public static class LEDPixelFont5x5
{
    public static Dictionary<char, int[,]> CreateFont()
    {
        var font = new Dictionary<char, int[,]>();

        font['A'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 } };
        font['B'] = new int[,] { { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 } };
        font['C'] = new int[,] { { 0, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 } };
        font['D'] = new int[,] { { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 } };
        font['E'] = new int[,] { { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 1 } };
        font['F'] = new int[,] { { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 } };
        font['G'] = new int[,] { { 0, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 1, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 1 } };
        font['H'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 } };
        font['I'] = new int[,] { { 1, 1, 1, 1, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 1, 1, 1, 1, 1 } };
        font['J'] = new int[,] { { 1, 1, 1, 1, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 1, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 } };
        font['K'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 0, 0, 1, 0 }, { 1, 1, 0, 0, 0 }, { 1, 0, 0, 1, 0 }, { 1, 0, 0, 0, 1 } };
        font['L'] = new int[,] { { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 1 } };
        font['M'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 1, 0, 1, 1 }, { 1, 0, 1, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 } };
        font['N'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 1, 0, 0, 1 }, { 1, 0, 1, 0, 1 }, { 1, 0, 0, 1, 1 }, { 1, 0, 0, 0, 1 } };
        font['O'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };
        font['P'] = new int[,] { { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 } };
        font['Q'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 1, 0, 1 }, { 0, 1, 1, 0, 1 } };
        font['R'] = new int[,] { { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 }, { 1, 0, 1, 0, 0 }, { 1, 0, 0, 0, 1 } };
        font['S'] = new int[,] { { 0, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 } };
        font['T'] = new int[,] { { 1, 1, 1, 1, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 } };
        font['U'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };
        font['V'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 0, 1, 0 }, { 0, 0, 1, 0, 0 } };
        font['W'] = new int[,] { { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 1, 0, 1, 0, 1 }, { 1, 0, 1, 0, 1 }, { 0, 1, 0, 1, 0 } };
        font['X'] = new int[,] { { 1, 0, 0, 0, 1 }, { 0, 1, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 0, 1, 0 }, { 1, 0, 0, 0, 1 } };
        font['Y'] = new int[,] { { 1, 0, 0, 0, 1 }, { 0, 1, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 } };
        font['Z'] = new int[,] { { 1, 1, 1, 1, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 1, 1, 1, 1, 1 } };

        font['0'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 1, 1 }, { 1, 0, 1, 0, 1 }, { 1, 1, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };
        font['1'] = new int[,] { { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 1, 0 } };
        font['2'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 1, 1, 1, 1, 1 } };
        font['3'] = new int[,] { { 1, 1, 1, 1, 0 }, { 0, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 } };
        font['4'] = new int[,] { { 0, 0, 1, 1, 0 }, { 0, 1, 0, 1, 0 }, { 1, 0, 0, 1, 0 }, { 1, 1, 1, 1, 1 }, { 0, 0, 0, 1, 0 } };
        font['5'] = new int[,] { { 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 0 }, { 0, 0, 0, 0, 1 }, { 1, 1, 1, 1, 0 } };
        font['6'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };
        font['7'] = new int[,] { { 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 0, 0, 0 } };
        font['8'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };
        font['9'] = new int[,] { { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0 } };

        font[' '] = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        font['.'] = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 } };
        font[','] = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 0, 0, 0 } };
        font[':'] = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
        font['!'] = new int[,] { { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 } };

        return font;
    }
}