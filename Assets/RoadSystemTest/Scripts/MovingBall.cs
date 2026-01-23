using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una bolita que se mueve a través del sistema de carreteras
/// </summary>
public class MovingBall : MonoBehaviour
{
    public float speed = 3f;            // Velocidad de movimiento de la bolita
    public GameObject deathParticle;    // Prefab de particulas de destrucción de la bolita

    private Vector3Int currentCell;     // Celda en la cual se encuentra la bolita actualmente
    private RoadDirection direction;    // Dirección en la que la bolita se está moviendo
    private GridSystem grid;            // Referencia al sistema de la grid

    /// <summary>
    /// Inicializa la bolita cuando se crea
    /// </summary>
    /// <param name="startCell">Celda inicial de la bolita</param>
    /// <param name="startDirection">Dirección inicial de la bolita</param>
    public void Initialize(Vector3Int startCell, RoadDirection startDirection)
    {
        // Guardamos los parámetros de celda y dirección iniciales
        currentCell = startCell;
        direction = startDirection;
        grid = FindObjectOfType<GridSystem>();
    }

    private void Update()
    {
        // Hace que se mueva la bolita en cada frame de juego
        Move();
    }

    /// <summary>
    /// Método que mueve la bolita hacia el centro de la celda actual
    /// </summary>
    void Move()
    {
        // Comprueba si en la celda actual existe una carretera
        if (grid.placedObjects.ContainsKey(currentCell))
        {
            // Obtiene la pieza de la carretera actual
            RoadPiece piece = grid.placedObjects[currentCell].GetComponent<RoadPiece>();
            // Calcula la dirección desde la que la bolita está entrando
            RoadDirection incoming = Opposite(direction);
            // Prepara una variable para comprobar si la entrada es válida
            bool acceptsInput = false;

            // Recorre las conexiones de la carretera, si alguna coincide con la dirección de entrada, la carretera acepta la bolita
            foreach (var c in piece.connections)
            {
                if (c == incoming)
                {
                    acceptsInput = true;
                    break;
                }
            }

            // Si la carretera no acepta entrada desde esa dirección, la bolita se destruye inmediatamente, representando un "choque" contra la pared de la carretera
            if (!acceptsInput)
            {
                Destroy(gameObject);
                return;
            }
        }    

        // Calcula la posición central de la celda actual
        Vector3 targetPos = currentCell;

        // Mueve la bolita suavemente hacia el centro de la celda
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Cuando llega al centro de la celda, decide hacia dónde tiene que ir
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            AdvanceToNextCell();
        }
    }

    /// <summary>
    /// Decide hacia dónde tiene que ir la bolita después de llegar al centro de la celda actual
    /// </summary>
    void AdvanceToNextCell()
    {
        // Busca la pieza actual, si es una salida, ejecuta la salida de la carretera, si no, destruye la bolita
        if (!grid.placedObjects.ContainsKey(currentCell))
        {
            if (grid.outputs.ContainsKey(currentCell))
                grid.outputs[currentCell].ReceiveBall();
            Destroy(gameObject);
            return;
        }

        // Calcula la siguiente celda
        Vector3Int next = NextCell(currentCell, direction);

        // Obtiene la pieza de la carretera actual
        RoadPiece piece = grid.placedObjects[currentCell].GetComponent<RoadPiece>();

        // Determina la dirección de salida
        RoadDirection nextDir = FindNextDirection(piece);

        // Actualiza la dirección de movimiento i avanza la bolita a la siguiente celda
        direction = nextDir;
        currentCell = NextCell(currentCell, direction);
    }

    /// <summary>
    /// Decide la salida de la bolita en la carretera proporcionada
    /// </summary>
    /// <param name="piece">Pieza de la carretera a calcular la dirección de salida</param>
    /// <returns>La dirección hacia la cual la bolita debe dirigirse</returns>
    RoadDirection FindNextDirection(RoadPiece piece)
    {
        // Crea una lista de salidas válidas i calcula la dirección de entrada
        List<RoadDirection> validOutputs = new List<RoadDirection>();
        RoadDirection incoming = Opposite(direction);

        // Añade todas las conexiones que tiene la pieza salvo la entrada a la lista
        foreach (var c in piece.connections)
        {
            if (c != incoming)
                validOutputs.Add(c);
        }

        // Si no hay salidas válidas, la bolita debe morir
        if (validOutputs.Count == 0)
            return (RoadDirection)(-1);

        // Elegir salida según round-robin, distribuyendo el flujo por cada una de las salidas
        int index = piece.nextOutputIndex % validOutputs.Count;
        RoadDirection chosen = validOutputs[index];
        piece.nextOutputIndex++;

        return chosen;
    }

    /// <summary>
    /// Calcula la celda adyacente según la dirección
    /// </summary>
    /// <param name="cell">Posición de la celda actual</param>
    /// <param name="dir">Dirección de la bolita actual</param>
    /// <returns>La posición de la celda hacia la que se moverá la bolita</returns>
    Vector3Int NextCell(Vector3Int cell, RoadDirection dir)
    {
        // Calcula la celda adyacente en la dirección de salida i la devuelve
        switch (dir)
        {
            case RoadDirection.Up: return cell + new Vector3Int(0, 0, 1);
            case RoadDirection.Down: return cell + new Vector3Int(0, 0, -1);
            case RoadDirection.Left: return cell + new Vector3Int(-1, 0, 0);
            case RoadDirection.Right: return cell + new Vector3Int(1, 0, 0);
        }
        return cell;
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

    private void OnDestroy()
    {
        // Cuando la bolita se destruye, instancia un efecto visual en su posición
        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}