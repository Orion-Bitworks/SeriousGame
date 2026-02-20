using UnityEngine;

/// <summary>
/// Gestiona el funcionamiento del "fantasma" que indica la pieza que vas a colocar
/// </summary>
public class GhostController : MonoBehaviour
{
    public static GhostController Instance { get; private set; }    // Referencia Singleton

    [HideInInspector] public GameObject ghostObject;                                  // Objeto fantasma que sigue al ratón, ghost
    [HideInInspector] public Quaternion currentRotation = Quaternion.identity;        // Rotación actual del ghost

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    /// <summary>
    /// Instancia el ghost en escena
    /// </summary>
    public void CreateGhost(GameObject prefab)
    {
        // Si ya existe, lo destruye
        if (ghostObject != null)
            Destroy(ghostObject);

        // Lo instancia y desactiva su collider para evitar colisiones
        ghostObject = Instantiate(prefab);
        ghostObject.GetComponent<Collider>().enabled = false;

        // Lo hace transparente y le actualiza la rotación a la actual
        MakeTransparent(ghostObject);
        ghostObject.transform.rotation = currentRotation;
    }

    /// <summary>
    /// Actualiza la posición del ghost según las indicaciones que recibe
    /// </summary>
    /// <param name="snapped">La posición a la que el ghost debe moverse</param>
    /// <param name="occupied">True si la celda está ocupada, false si no</param>
    /// <param name="insideBounds">True si se encuentra dentro del mapa, false si no</param>
    public void UpdateGhostPosition(Vector3Int snapped, bool occupied, bool insideBounds)
    {
        ghostObject.transform.position = snapped;

        // Cambia el color del fantasma: Rojo -> Celda ocupada, Azul -> Fuera de límites, Blanco -> Válido
        if (!insideBounds)
            SetColor(Color.blue);
        else if (occupied)
            SetColor(Color.red);
        else
            SetColor(new Color(1f, 1f, 1f, 0.5f));
    }

    /// <summary>
    /// Rota el ghost 90º
    /// </summary>
    public void RotateGhost()
    {
        // Rota 90º en el eje Y, y actualiza el ghost
        currentRotation *= Quaternion.Euler(0f, 90f, 0f);
        ghostObject.transform.rotation = currentRotation;

        // Si no es carretera, no hay conexiones que rotar
        RoadPiece road = ghostObject.GetComponent<RoadPiece>();
        if (road == null || road.connections == null) return;

        // Rota las conexiones lógicamente
        for (int i = 0; i < road.connections.Length; i++)
        {
            road.connections[i] = DirectionUtils.Rotate90(road.connections[i]);
        }
    }

    /// <summary>
    /// Hace que el objeto recibido sea semitransparente
    /// </summary>
    /// <param name="obj">El objeto a hacer semitransparente</param>
    private void MakeTransparent(GameObject obj)
    {
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
        {
            Material mat = r.material;
            Color c = mat.color;
            c.a = 0.5f;
            mat.color = c;
        }
    }

    /// <summary>
    /// Le cambia el color al ghost
    /// </summary>
    /// <param name="color">Color al que tenemos que cambiar al ghost</param>
    private void SetColor(Color color)
    {
        foreach (Renderer r in ghostObject.GetComponentsInChildren<Renderer>())
            r.material.color = color;
    }
}
