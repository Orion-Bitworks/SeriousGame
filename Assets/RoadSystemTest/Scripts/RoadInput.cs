using System.Collections;
using UnityEngine;

/// <summary>
/// Representa un punto de entrada que genera bolitas hacia el sistema de carreteras
/// </summary>
public class RoadInput : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;                     // Apunta al prefab de la bolita que se va a instanciar
    [SerializeField] float spawnRate = 1f;                      // Tiempo entre cada bolita generada
    [SerializeField] public RoadDirection outputDirection;      // Dirección en la cual se envian las bolitas
    [SerializeField] BallType ballTypeToSpawn;                  // Tipo de bolita que queremos que aparezca

    private void OnEnable()
    {
        // Inicia una corutina que genera bolitas periódicamente
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Empieza a generar bolitas
    /// </summary>
    public void StartGenerating()
    {
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Deja de generar bolitas
    /// </summary>
    public void StopGenerating()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    /// <summary>
    /// Intenta generar bolitas periódicamente
    /// </summary>
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Espera un tiempo definido
            yield return new WaitForSeconds(spawnRate / GameManager.Instance.velocityMultiplier);
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

        // Instancia la bolita y le cambia el color al correcto
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        ball.GetComponentInChildren<Renderer>().material = BallTypeMaterials.GetMaterial(ballTypeToSpawn);

        // Inicializa el movimiento de la bolita, indicando tanto a que celda debe ir como la dirección que debe seguir
        ball.GetComponent<MovingBall>().Initialize(nextCell, outputDirection, ballTypeToSpawn);
    }
}