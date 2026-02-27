using UnityEngine;

/// <summary>
/// Se encarga de registar todas las tuberías internas del corazón dentro de la grid
/// </summary>
public class InternalPipeRegister : MonoBehaviour
{
    GridManager grid;   // Referencia al "GridManager"

    /// <summary>
    /// Guarda la referencia al grid y ejecuta el registro de las tuberías internas
    /// </summary>
    /// <param name="grid"></param>
    public void Register(GridManager grid)
    {
        if (this.grid != null)
            return;

        this.grid = grid;
        RegisterInternalPipes();
    }

    /// <summary>
    /// Registra tanto tuberías como outputs en los diccionarios correspondientes
    /// </summary>
    void RegisterInternalPipes()
    {
        // Buscamos todas las tuberías y outputs
        RoadPiece[] pipes = GetComponentsInChildren<RoadPiece>();
        RoadOutput[] outputs = GetComponentsInChildren<RoadOutput>();

        // Para cada tubería, calcula su celda y la registra en su diccionario
        foreach (var pipe in pipes)
        {
            Vector3Int cell = Vector3Int.RoundToInt(pipe.transform.position);
            grid.placedObjects[cell] = pipe.gameObject;
        }

        // Para cada output, calcula su celda y lo registra en su diccionario
        foreach (var output in outputs)
        {
            Vector3Int cell = Vector3Int.RoundToInt(output.transform.position);
             grid.outputs[cell] = output;
        }
    }

    /// <summary>
    /// Elimina tanto tuberías como outputs en los diccionarios correspondientes
    /// </summary>
    /// <param name="grid"></param>
    public void Unregister(GridManager grid)
    {
        // Para cada tubería, calcula su celda y la elimina de su diccionario
        foreach (var pipe in GetComponentsInChildren<RoadPiece>())
        {
            Vector3Int cell = Vector3Int.RoundToInt(pipe.transform.position);
            grid.placedObjects.Remove(cell);
        }

        // Para cada output, calcula su celda y la elimina de su diccionario
        foreach (var output in GetComponentsInChildren<RoadOutput>())
        {
            Vector3Int cell = Vector3Int.RoundToInt(output.transform.position);
            grid.outputs.Remove(cell);
        }
    }
}
