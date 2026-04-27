using UnityEngine;

/// <summary>
/// Representa las cuatro direcciones cardinales que una pieza puede tener
/// </summary>
public enum RoadDirection
{
    Up,
    Right,
    Down,
    Left
}

/// <summary>
/// Representa una pieza colocada en la grid
/// </summary>
public class RoadPiece : MonoBehaviour
{
    public RoadDirection[] connections;     // Vector con las direcciones por las que la pieza puede estar conectada
    public int nextOutputIndex = 0;         // Índice usado para el "round-robin" en piezas con múltiples salidas, es decir, distribuye el flujo por cada una de las salidas
    public int requiredExits = 1;
    public bool[] entryUsed;
    public bool[] exitUsed;
    public bool wasUsed;

    void Awake()
    {
        entryUsed = new bool[connections.Length];
        exitUsed = new bool[connections.Length];
    }
}