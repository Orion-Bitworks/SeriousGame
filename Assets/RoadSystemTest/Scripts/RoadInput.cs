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

    private GridManager grid;                // Referencia al sistema de la grid

    private void Start()
    {
        grid = GridManager.Instance;
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
            if (GameManager.Instance.isPlaying)
                TrySpawnBall();
        }
    }

    /// <summary>
    /// Intenta generar una bolita y ponerla en movimiento en la grid
    /// </summary>
    void TrySpawnBall()
    {
        // Calcula la celda hacia la que saldrá la bolita
        Vector3Int nextCell = Vector3Int.RoundToInt(transform.position) + DirectionUtils.ToVector(outputDirection);

        // Instancia la bolita
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

        // Inicializa el movimiento de la bolita, indicando tanto a que celda debe ir como la dirección que debe seguir
        ball.GetComponent<MovingBall>().Initialize(nextCell, outputDirection);
    }

    /// <summary>
    /// Calcula si la siguiente celda tiene una entrada válida para poder generar la bolita
    /// </summary>
    /// <param name="piece">La pieza sobre la cual se va a generar la bolita</param>
    /// <returns>True si se puede generar, false si no</returns>
    bool IsConnectionValid(RoadPiece piece)
    {
        // Calcula la dirección desde la que la carretera debería aceptar entrada
        RoadDirection opposite = DirectionUtils.Opposite(outputDirection);

        // Recorre las conexiones de la carretera, si alguna coincide con la dirección opuesta calculada, la entrada es válida
        foreach (var c in piece.connections)
        {
            if (c == opposite)
                return true;
        }

        return false;
    }
}