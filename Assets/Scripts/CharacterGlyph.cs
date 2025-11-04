using UnityEngine;

// [System.Serializable] permite que esta clase se muestre
// y edite en el Inspector de Unity dentro de otra clase.
[System.Serializable]
public class CharacterGlyph
{
    public char character;

    // Usamos un array plano (bool[]) porque Unity
    // no puede serializar (mostrar) arrays 2D (bool[,]).
    // Lo editaremos como una lista larga.
    public bool[] glyphData;
}