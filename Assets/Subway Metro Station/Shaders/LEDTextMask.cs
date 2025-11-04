using UnityEngine;
using System.Collections.Generic;

public class LEDTextRuntime : MonoBehaviour
{
    [Header("Shader Material")]
    public Material ledMaterial;   // Material con tu shader LED

    [Header("Fuente Pixelada")]
    public int charWidth = 5;      // ancho base de cada letra
    public int charHeight = 7;     // alto base de cada letra
    public int spacing = 1;        // espacio entre letras
    public int scale = 2;          // factor de escala (5x7 → 10x14 aprox.)

    [Header("Cartel LED")]
    public int textureWidth = 64;  // ancho total del panel
    public int textureHeight = 14; // alto total del panel

    private Texture2D maskTexture;
    private Dictionary<char, int[,]> fontMap;
    [SerializeField]
    private string defaulMessage = string.Empty;

    void Start()
    {
        // Inicializar fuente pixelada
        fontMap = LEDPixelFont5x7.CreateFont();

        // Crear textura inicial
        maskTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Point;
        maskTexture.wrapMode = TextureWrapMode.Repeat;

        // Mensaje inicial
        GenerarTexto(defaulMessage);
    }

    public void GenerarTexto(string mensaje)
    {
        // Calcular ancho dinámico
        int totalWidth = mensaje.Length * (charWidth * scale + spacing);

        // Crear nueva textura con el ancho necesario
        maskTexture = new Texture2D(totalWidth, textureHeight, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Point;
        maskTexture.wrapMode = TextureWrapMode.Clamp; // evita unión de palabras

        Color32[] pixels = new Color32[maskTexture.width * maskTexture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;

        int cursorX = 0;

        foreach (char c in mensaje.ToUpper())
        {
            if (!fontMap.ContainsKey(c))
            {
                cursorX += (charWidth * scale) + spacing;
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
                                int py = (maskTexture.height - 1 - (y * scale + sy));

                                if (px < maskTexture.width && py < maskTexture.height)
                                    pixels[py * maskTexture.width + px] = Color.white;
                            }
                        }
                    }
                }
            }

            cursorX += (charWidth * scale) + spacing;
        }

        maskTexture.SetPixels32(pixels);
        maskTexture.Apply();

        ledMaterial.SetTexture("_MaskTex", maskTexture);
    }
} 

public static class LEDPixelFont5x7
{
    public static Dictionary<char, int[,]> CreateFont()
    {
        var font = new Dictionary<char, int[,]>();
        // A
        font['A'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1}
        };

        // B
        font['B'] = new int[,]
        {
            {1,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,0}
        };

        // C
        font['C'] = new int[,]
        {
            {0,1,1,1,1},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {0,1,1,1,1}
        };

        // D
        font['D'] = new int[,]
        {
            {1,1,1,0,0},
            {1,0,0,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,1,0},
            {1,1,1,0,0}
        };

        // E
        font['E'] = new int[,]
        {
            {1,1,1,1,1},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,1}
        };

        // F
        font['F'] = new int[,]
        {
            {1,1,1,1,1},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0}
        };

        // G
        font['G'] = new int[,]
        {
            {0,1,1,1,1},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,1,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,1}
        };

        // H
        font['H'] = new int[,]
        {
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1}
        };

        // I
        font['I'] = new int[,]
        {
            {1,1,1,1,1},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {1,1,1,1,1}
        };

        // J
        font['J'] = new int[,]
        {
            {0,0,0,1,1},
            {0,0,0,0,1},
            {0,0,0,0,1},
            {0,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // K
        font['K'] = new int[,]
        {
            {1,0,0,0,1},
            {1,0,0,1,0},
            {1,0,1,0,0},
            {1,1,0,0,0},
            {1,0,1,0,0},
            {1,0,0,1,0},
            {1,0,0,0,1}
        };

        // L
        font['L'] = new int[,]
        {
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,1}
        };

        // M
        font['M'] = new int[,]
        {
            {1,0,0,0,1},
            {1,1,0,1,1},
            {1,0,1,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1}
        };

        // N
        font['N'] = new int[,]
        {
            {1,0,0,0,1},
            {1,1,0,0,1},
            {1,0,1,0,1},
            {1,0,0,1,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1}
        };

        // O
        font['O'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // P
        font['P'] = new int[,]
        {
            {1,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,0,0,0,0}
        };

        // Q
        font['Q'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,1,0,1},
            {1,0,0,1,0},
            {0,1,1,0,1}
        };

        // R
        font['R'] = new int[,]
        {
            {1,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,1,1,1,0},
            {1,0,1,0,0},
            {1,0,0,1,0},
            {1,0,0,0,1}
        };

        // S
        font['S'] = new int[,]
        {
            {0,1,1,1,1},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {0,1,1,1,0},
            {0,0,0,0,1},
            {0,0,0,0,1},
            {1,1,1,1,0}
        };

        // T
        font['T'] = new int[,]
        {
            {1,1,1,1,1},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0}
        };

        // U
        font['U'] = new int[,]
        {
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // V
        font['V'] = new int[,]
        {
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,0,1,0},
            {0,1,0,1,0},
            {0,0,1,0,0}
        };

        // W
        font['W'] = new int[,]
        {
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,1,0,1},
            {1,0,1,0,1},
            {1,1,0,1,1},
            {1,0,0,0,1}
        };

        // X
        font['X'] = new int[,]
        {
            {1,0,0,0,1},
            {0,1,0,1,0},
            {0,1,0,1,0},
            {0,0,1,0,0},
            {0,1,0,1,0},
            {0,1,0,1,0},
            {1,0,0,0,1}
        };

        // Y
        font['Y'] = new int[,]
        {
            {1,0,0,0,1},
            {0,1,0,1,0},
            {0,1,0,1,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0}
        };

        // Z
        font['Z'] = new int[,]
        {
            {1,1,1,1,1},
            {0,0,0,0,1},
            {0,0,0,1,0},
            {0,0,1,0,0},
            {0,1,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,1}
        };

        // 0
        font['0'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,1,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // 1
        font['1'] = new int[,]
        {
            {0,0,1,0,0},
            {0,1,1,0,0},
            {1,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {0,0,1,0,0},
            {1,1,1,1,1}
        };

        // 2
        font['2'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {0,0,0,0,1},
            {0,0,0,1,0},
            {0,0,1,0,0},
            {0,1,0,0,0},
            {1,1,1,1,1}
        };

        // 3
        font['3'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {0,0,0,0,1},
            {0,0,1,1,0},
            {0,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // 4
        font['4'] = new int[,]
        {
            {0,0,0,1,0},
            {0,0,1,1,0},
            {0,1,0,1,0},
            {1,0,0,1,0},
            {1,1,1,1,1},
            {0,0,0,1,0},
            {0,0,0,1,0}
        };

        // 5
        font['5'] = new int[,]
        {
            {1,1,1,1,1},
            {1,0,0,0,0},
            {1,1,1,1,0},
            {0,0,0,0,1},
            {0,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // 6
        font['6'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,0},
            {1,0,0,0,0},
            {1,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // 7
        font['7'] = new int[,]
        {
            {1,1,1,1,1},
            {0,0,0,0,1},
            {0,0,0,1,0},
            {0,0,1,0,0},
            {0,1,0,0,0},
            {0,1,0,0,0},
            {0,1,0,0,0}
        };

        // 8
        font['8'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,0}
        };

        // 9
        font['9'] = new int[,]
        {
            {0,1,1,1,0},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {0,1,1,1,1},
            {0,0,0,0,1},
            {0,0,0,0,1},
            {0,1,1,1,0}
        };


        return font;
    }
}