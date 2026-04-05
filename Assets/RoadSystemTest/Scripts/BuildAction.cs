using UnityEngine;

/// <summary>
/// Define los tres tipos de acciones que pueden ocurrir en el sistema de construcción
/// </summary>
public enum BuildActionType
{
    Place,
    Erase,
    OrganPlace
}

/// <summary>
/// Registro completo de una acción concreta que el jugador ha hecho, y son las que se guardan en las pilas de Undo/Redo
/// </summary>
public class BuildAction
{
    public BuildActionType type;            // Tipo de la acción
    public Vector3Int cell;                 // Posición en la grid
    public GameObject prefab;               // Prefab de la pieza
    public Quaternion rotation;             // Rotación de la pieza
    public RoadDirection[] connections;     // Conexiones de la pieza
    public OrganData organData;

    /// <summary>
    /// Constructor de la acción, asigna cada parámetro recibido al campo correspondiente
    /// </summary>
    /// <param name="type">Tipo de la acción</param>
    /// <param name="cell">Posición en la grid</param>
    /// <param name="prefab">Prefab de la pieza</param>
    /// <param name="rotation">Rotación de la pieza</param>
    /// <param name="connections">Conexiones de la pieza</param>
    public BuildAction(BuildActionType type, Vector3Int cell, GameObject prefab, Quaternion rotation, RoadDirection[] connections)
    {
        this.type = type;
        this.cell = cell;
        this.prefab = prefab;
        this.rotation = rotation;
        this.connections = (RoadDirection[])connections.Clone();
        this.organData = null;
    }

    public BuildAction(BuildActionType type, Vector3Int cell, GameObject prefab, Quaternion rotation, OrganData organData)
    {
        this.type = type;
        this.cell = cell;
        this.prefab = prefab;
        this.rotation = rotation;
        this.connections = null;
        this.organData = organData;
    }
}