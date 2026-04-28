using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum con los tipos de bolitas
/// </summary>
public enum BallType
{
    NotO2FromPipe,
    NotO2FromHeart,
    O2FromPipe,
    O2FromHeart,
    Red,
    Blue,
    Green,
    Yellow
}

public static class BallTypeMaterials
{
    private static readonly Dictionary<BallType, Material> materials = new();

    public static void RegisterMaterial(BallType type, Material mat)
    {
        materials[type] = mat;
    }

    public static Material GetMaterial(BallType type)
    {
        return materials[type];
    }
}

/// <summary>
/// Representa una bolita que se mueve a través del sistema de carreteras
/// </summary>
public class MovingBall : MonoBehaviour
{
    [SerializeField] float speed = 3f;              // Velocidad de movimiento de la bolita
    [SerializeField] GameObject deathParticle;      // Prefab de particulas de destrucción de la bolita
    [SerializeField] public BallType ballType;      // Tipo inicial de la bolita

    Vector3Int currentCell;     // Celda en la cual se encuentra la bolita actualmente
    RoadDirection direction;    // Dirección en la que la bolita se está moviendo
    GridManager grid;           // Referencia al sistema de la grid

    /// <summary>
    /// Inicializa la bolita cuando se crea
    /// </summary>
    /// <param name="startCell">Celda inicial de la bolita</param>
    /// <param name="startDirection">Dirección inicial de la bolita</param>
    /// <param name="type">Tipo inicial de la bolita</param>
    public void Initialize(Vector3Int startCell, RoadDirection startDirection, BallType type)
    {
        // Guardamos los parámetros de celda, dirección y tipo iniciales
        currentCell = startCell;
        direction = startDirection;
        ballType = type;
        grid = GridManager.Instance;
    }

    private void Update()
    {
        // Hace que se mueva la bolita en cada frame de juego
        Move();
    }

    /// <summary>
    /// Mueve la bolita hacia el centro de la celda actual
    /// </summary>
    void Move()
    {
        // Comprueba si la bolita puede entrar hacia la siguiente carretera
        if (!CanEnterCurrentCell())
        {
            // Si no puede, destruye la bolita
            DestroyBall();
            return;
        }

        // Si puede, se mueve hacia la misma
        MoveTowardsCurrentCell();

        // Cuando llega al centro de la celda, decide hacia dónde tiene que ir
        if (ReachedCurrentCell())
            AdvanceToNextCell();
    }

    /// <summary>
    /// Decide hacia dónde tiene que ir la bolita después de llegar al centro de la celda actual
    /// </summary>
    void AdvanceToNextCell()
    {
        // Si es un output, intenta que la bolita acabe su recorrido
        if (IsOutputCell(currentCell))
        {
            TryDeliverToOutput(currentCell);
            Destroy(gameObject);
            return;
        }

        // Si no hay carretera en esta celda, destruyela
        if (!grid.placedObjects.ContainsKey(currentCell))
        {
            {
                DestroyBall();
                return;
            }
        }

        // Obtiene la pieza de la carretera actual y determina la dirección de salida
        RoadPiece piece = GetCurrentPiece();
        RoadDirection nextDir = GetNextDirection(piece);

        // Si no existe salida, se destruye la bolita
        if (nextDir == (RoadDirection)(-1))
        {
            DestroyBall();
            return;
        }

        // Actualiza la dirección de movimiento i avanza la bolita a la siguiente celda
        direction = nextDir;
        currentCell += DirectionUtils.ToVector(direction);
    }

    /// <summary>
    /// Comprueba si la bolita puede entrar a la celda actual
    /// </summary>
    /// <returns>True si puede, false si no</returns>
    bool CanEnterCurrentCell()
    {
        // Comprueba si en la celda actual existe una carretera
        if (!grid.placedObjects.ContainsKey(currentCell))
            return true;

        // Obtiene la pieza de la carretera actual 
        RoadPiece piece = grid.placedObjects[currentCell].GetComponent<RoadPiece>();

        // Abortamos si la pieza no existe o si no tiene conexiones
        if (piece == null)
        {
            Debug.Log("La pieza no existe");
            return false;
        }
        
        if (piece.connections == null || piece.connections.Length == 0)
        {
            Debug.Log("La pieza no tiene conexiones");
            return false;
        }

        // Calcula la dirección desde la que la bolita está entrando
        RoadDirection incoming = DirectionUtils.Opposite(direction);

        int entryIndex = System.Array.IndexOf(piece.connections, incoming);

        // Si entra por un lado que ya fue usado como salida, error
        if (entryIndex >= 0 && piece.exitUsed[entryIndex])
        {
            DestroyBall();
        }

        // Marcar entrada usada
        if (entryIndex >= 0)
        {
            piece.entryUsed[entryIndex] = true;
        }

        // Recorre las conexiones de la carretera, si alguna coincide con la dirección de entrada, la carretera acepta la bolita
        foreach (var c in piece.connections)
            if (c == incoming)
                return true;

        return false;
    }

    /// <summary>
    /// Comprueba si la celda recibida es una salida
    /// </summary>
    /// <param name="cell">Celda a comprobar</param>
    /// <returns>True si es una salida, false si no</returns>
    bool IsOutputCell(Vector3Int cell)
    {
        return grid.outputs.ContainsKey(cell);
    }

    /// <summary>
    /// Intenta entregar la bolita en un punto de salida
    /// </summary>
    /// <param name="cell">Celda a comprobar si es un punto de salida</param>
    void TryDeliverToOutput(Vector3Int cell)
    {
        grid.outputs[currentCell].ReceiveBall(this);
    }

    /// <summary>
    /// Obtiene la carretera contenida en la celda actual
    /// </summary>
    /// <returns>La carretera actual</returns>
    RoadPiece GetCurrentPiece()
    {
        //Comprueba si existe una carretera en la celda actual i devuelve dicha pieza si existe
        if (!grid.placedObjects.ContainsKey(currentCell))
            return null;

        return grid.placedObjects[currentCell].GetComponent<RoadPiece>();
    }

    /// <summary>
    /// Decide la salida de la bolita en la carretera proporcionada
    /// </summary>
    /// <param name="piece">Pieza de la carretera a calcular la dirección de salida</param>
    /// <returns>La dirección hacia la cual la bolita debe dirigirse</returns>
    RoadDirection GetNextDirection(RoadPiece piece)
    {
        // Crea una lista de salidas válidas i calcula la dirección de entrada
        List<RoadDirection> valid = new List<RoadDirection>();
        RoadDirection incoming = DirectionUtils.Opposite(direction);

        // Añade todas las conexiones que tiene la pieza, salvo la entrada, a la lista
        foreach (var c in piece.connections)
            if (c != incoming)
                valid.Add(c);

        // Si no hay salidas válidas, la bolita debe morir
        if (valid.Count == 0)
            return (RoadDirection)(-1);

        // Elige salida según "round-robin", distribuyendo el flujo por cada una de las salidas
        int index = piece.nextOutputIndex % valid.Count;
        piece.nextOutputIndex++;

        RoadDirection chosen = valid[index];

        // Marcamos la salida utilizada como usada
        int originalIndex = System.Array.IndexOf(piece.connections, chosen);
        if (originalIndex >= 0)
            piece.exitUsed[originalIndex] = true;

        // Si esta salida también fue usada como entrada, error
        if (originalIndex >= 0 && piece.entryUsed[originalIndex])
        {
            DestroyBall();
        }

        return chosen;
    }

    /// <summary>
    /// Destruye la bolita
    /// </summary>
    public void DestroyBall()
    {
        // Cuando la bolita se destruye, instancia un efecto visual en su posición
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Debug.Log("La bolita no ha llegado a ninguna parte");
        GameManager.Instance.failed = true;
        Destroy(gameObject);
    }

    /// <summary>
    /// Mueve la bolita hasta el centro de la celda actual
    /// </summary>
    void MoveTowardsCurrentCell()
    {
        // Calcula la posición central de la celda actual
        Vector3 targetPos = currentCell;
        // Mueve la bolita suavemente hacia el centro de la celda
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * GameManager.Instance.velocityMultiplier * Time.deltaTime);
    }

    /// <summary>
    /// Comprueba la dirección de la bolita respecto al centro de la celda
    /// </summary>
    /// <returns>True si está justo en el centro, false si no</returns>
    bool ReachedCurrentCell()
    {
        return Vector3.Distance(transform.position, currentCell) < 0.01f;
    }
}