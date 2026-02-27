using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Se encarga de calcular el número de celdas exacto que ocupa una pieza grande en la grid
/// </summary>
public class MultiCellPiece : MonoBehaviour
{
    public Vector3Int[] occupiedOffsets;    // Array que almacena los offsets locales que ocupa la pieza respecto a su posición base

    // Calculamos todos los offsets al inicializar el objeto
    private void Awake()
    {
        GenerateOffsetsFromChildren();
    }

    /// <summary>
    /// Genera todos los offsets de la pieza a partir de los colliders colocados en el prefab de la misma
    /// </summary>
    private void GenerateOffsetsFromChildren()
    {
        // Obtenemos todos los colliders que representan la pieza, cada uno representa una celda ocupada
        // ¡IMPORTANTE! AL CREAR OTRA PIEZA MULTICELDA, RECORDAR COLOCAR UN BOX COLLIDER DE 1X1X1 EN CADA ESPACIO QUE OCUPA LA PIEZA
        var colliders = GetComponentsInChildren<BoxCollider>();
        // Guardamos los offsets en un hash para evitar duplicados
        HashSet<Vector3Int> offsets = new HashSet<Vector3Int>();

        // Recorremos todos los colliders para convertirlos a coordenadas de grid
        foreach (var col in colliders)
        {
            // Posición local del collider respecto al pivot del corazón
            Vector3 local = transform.InverseTransformPoint(col.transform.position);

            // Convertimos a coordenadas de grid
            Vector3Int cell = new Vector3Int(
                Mathf.RoundToInt(local.x),
                0,
                Mathf.RoundToInt(local.z)
            );

            // Lo añadimos al hash
            offsets.Add(cell);
        }

        // Lo guardamos en el array convirtiéndolo en una lista previamente
        occupiedOffsets = new List<Vector3Int>(offsets).ToArray();
    }
}