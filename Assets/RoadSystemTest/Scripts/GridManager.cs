using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }    // Lo hacemos Singleton

    public float gridSize = 1f;                                 // Tamaño de las celdas de la grid

    public Vector2Int minBounds = new Vector2Int(-14, -8);      // Límite del área mínima donde se pueden colocar piezas
    public Vector2Int maxBounds = new Vector2Int(14, 8);        // Límite del área máxima donde se pueden colocar piezas

    public Dictionary<Vector3Int, GameObject> placedObjects = new Dictionary<Vector3Int, GameObject>();     // Diccionario de piezas colocadas
    public Dictionary<Vector3Int, RoadOutput> outputs = new Dictionary<Vector3Int, RoadOutput>();           // Diccionario de salidas colocadas

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ajusta la posición al grid
    /// </summary>
    /// <param name="pos">Posición proporcionada para ajustar</param>
    /// <returns>La posición ajustada correctamente</returns>
    public Vector3Int Snap(Vector3 pos)
    {
        return new Vector3Int(
            Mathf.RoundToInt(pos.x / gridSize),
            0,
            Mathf.RoundToInt(pos.z / gridSize)
        );
    }

    /// <summary>
    /// Comprueba si una celda se encuentra dentro del área permitida
    /// </summary>
    /// <param name="cell">Celda a comprobar</param>
    /// <returns>True si se encuentra dentro del área, false si no</returns>
    public bool IsInsideBounds(Vector3Int cell)
    {
        return cell.x >= minBounds.x &&
               cell.x <= maxBounds.x &&
               cell.z >= minBounds.y &&
               cell.z <= maxBounds.y;
    }

    /// <summary>
    /// Hace que una celda solo se pueda encontrar dentro de los límites establecidos
    /// </summary>
    /// <param name="cell">Celda a cambiar su posición</param>
    /// <returns>Posición final de la celda dentro de los límites</returns>
    public Vector3Int ClampToBounds(Vector3Int cell)
    {
        int x = Mathf.Clamp(cell.x, minBounds.x, maxBounds.x);
        int z = Mathf.Clamp(cell.z, minBounds.y, maxBounds.y);
        return new Vector3Int(x, 0, z);
    }
}
