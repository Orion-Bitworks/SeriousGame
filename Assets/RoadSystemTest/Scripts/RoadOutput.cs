using UnityEngine;

/// <summary>
/// Representa un punto de salida del sistema de carreteras
/// </summary>
public class RoadOutput : MonoBehaviour
{
    public RoadDirection inputDirection; // Dirección desde la cual pueden llegar las bolitas

    private void Start()
    {
        GridSystem grid = FindObjectOfType<GridSystem>();
        // Convierte la posición del punto de salida a coordenadas de celda
        Vector3Int cell = Vector3Int.RoundToInt(transform.position);

        // Registra el punto de salida en su diccionario correspondiente
        if (!grid.outputs.ContainsKey(cell))
            grid.outputs.Add(cell, this);
    }

    /// <summary>
    /// Lo que ocurre cuando una bolita llega al punto de salida
    /// </summary>
    public void ReceiveBall()
    {
        Debug.Log("Bolita recibida en el output!");
        // Añadir suma de puntos?
    }
}