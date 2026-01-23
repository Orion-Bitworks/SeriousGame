using System.Collections;
using UnityEngine;

/// <summary>
/// Representa un punto de entrada que genera bolitas hacia el sistema de carreteras
/// </summary>
public class RoadInput : MonoBehaviour
{
    public GameObject ballPrefab;           // Apunta al prefab de la bolita que se va a instanciar
    public float spawnRate = 1f;            // Tiempo entre cada bolita generada
    public RoadDirection outputDirection;   // Dirección en la cual se envian las bolitas

    private GridSystem grid;                // Referencia al sistema de la grid

    private void Start()
    {
        grid = FindObjectOfType<GridSystem>();
        // Inicia una corutina que genera bolitas periódicamente
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Intenta generar bolitas periódicamente
    /// </summary>
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Espera un tiempo definido
            yield return new WaitForSeconds(spawnRate);
            // Intenta generar dicha bolita
            TrySpawnBall();
        }
    }

    /// <summary>
    /// Intenta generar una bolita y ponerla en movimiento en la grid
    /// </summary>
    void TrySpawnBall()
    {
        // Calcula la celda hacia la que saldrá la bolita
        Vector3Int nextCell = GetNextCell();

        // Si no hay una carretera en la siguiente celda, no genera bolita
        if (!grid.placedObjects.ContainsKey(nextCell))
            return;

        // Obtiene la pieza de la carretera colocada en la siguiente celda
        RoadPiece piece = grid.placedObjects[nextCell].GetComponent<RoadPiece>();

        // Si no tiene "RoadPiece", aborta
        if (piece == null)
            return;

        // Si la carretera no acepta entrada desde la dirección en la que se va a generar, no genera bolita
        if (!IsConnectionValid(piece))
            return;

        // Instancia la bolita
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

        // Inicializa el movimiento de la bolita, indicando tanto a que celda debe ir como la dirección que debe seguir
        ball.GetComponent<MovingBall>().Initialize(nextCell, outputDirection);
    }

    /// <summary>
    /// Calcula cual será la celda hacia la que tiene que salir la bolita
    /// </summary>
    /// <returns>La posición de la siguiente celda en la grid</returns>
    Vector3Int GetNextCell()
    {
        // Convierte la posición del input a coordenadas de celda
        Vector3Int pos = Vector3Int.RoundToInt(transform.position);

        // Calcula la celda adyacente en la dirección de salida i la devuelve
        switch (outputDirection)
        {
            case RoadDirection.Up: return pos + new Vector3Int(0, 0, 1);
            case RoadDirection.Down: return pos + new Vector3Int(0, 0, -1);
            case RoadDirection.Left: return pos + new Vector3Int(-1, 0, 0);
            case RoadDirection.Right: return pos + new Vector3Int(1, 0, 0);
        }

        return pos;
    }

    /// <summary>
    /// Calcula si la siguiente celda tiene una entrada válida para poder generar la bolita
    /// </summary>
    /// <param name="piece">La pieza sobre la cual se va a generar la bolita</param>
    /// <returns>True si se puede generar, false si no</returns>
    bool IsConnectionValid(RoadPiece piece)
    {
        // Calcula la dirección desde la que la carretera debería aceptar entrada
        RoadDirection opposite = Opposite(outputDirection);

        // Recorre las conexiones de la carretera, si alguna coincide con la dirección opuesta calculada, la entrada es válida
        foreach (var c in piece.connections)
        {
            if (c == opposite)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Calcula la dirección opuesta usando aritmética
    /// </summary>
    /// <param name="dir">Dirección para la cual tenemos que calcular su opuesto</param>
    /// <returns>El opuesto de la dirección proporcionada</returns>
    RoadDirection Opposite(RoadDirection dir)
    {
        // Si le damos dirección "Up" (0), devuelve "Down" (2)
        return (RoadDirection)(((int)dir + 2) % 4);
    }
}