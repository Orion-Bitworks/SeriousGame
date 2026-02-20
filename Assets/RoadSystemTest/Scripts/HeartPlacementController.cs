using UnityEngine;

/// <summary>
/// Se encarga de gestionar la colocación del corazón en la grid
/// </summary>
public class HeartPlacementController : MonoBehaviour
{
    public static HeartPlacementController Instance { get; private set; }   // Referencia Singleton

    [SerializeField] GameObject heartPrefab;                // Referencia al prefab del corazón

    [HideInInspector] public bool isPlacingHeart = false;   // Indica si se está colocando el corazón

    GameObject ghost;   // Apunta al "fantasma" del corazón
    GridManager grid;   // Referencia al "GridManager"
    Camera cam;         // Cámara usada para los raycasts

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    // Inicializamos parámetros
    private void Start()
    {
        cam = Camera.main;
        grid = GridManager.Instance;
    }

    /// <summary>
    /// Creamos el ghost del corazón y activamos el modo colocación
    /// </summary>
    public void StartPlacingHeart()
    {
        // Si el corazón ya está colocado, abortamos
        if (GameManager.Instance.heartPlaced) return;

        isPlacingHeart = true;
        ghost = Instantiate(heartPrefab);
    }

    // Actualizamos la posición del ghost en cada frame e intentamos colocar el corazón cuando el usuario lo desee
    private void Update()
    {
        // Si no estamos colocando, abortamos
        if (!isPlacingHeart) return;

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
            PlaceHeart();
    }

    /// <summary>
    /// Actualiza la posición del ghost respecto a la posición del ratón en pantalla
    /// </summary>
    void UpdateGhostPosition()
    {
        // Creamos un rayo de la cámara al ratón
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Si el rayo golpea algo, ajustamos esa posición al grid y movemos el ghost
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int cell = grid.Snap(hit.point);
            ghost.transform.position = cell;

            // Comprobar si el corazón cabe
            bool valid = CanPlaceHeartAt(cell);
            // Cambiar color del ghost
            SetGhostColor(valid);
        }
    }

    /// <summary>
    /// Se encarga de la colocación del corazón en la grid
    /// </summary>
    void PlaceHeart()
    {
        // Obtiene la última celda donde se encuentra el ghost
        Vector3Int cell = Vector3Int.RoundToInt(ghost.transform.position);

        // Si no se puede colocar en esa posición, abortamos
        if (!CanPlaceHeartAt(cell)) return;

        // Instanciamos el corazón real y registramos sus tuberías en la grid
        GameObject heart = Instantiate(heartPrefab, cell, Quaternion.identity);
        var reg = heart.GetComponent<InternalPipeRegister>();
        reg.Register(GridManager.Instance);

        // Marcamos que el corazón ya ha sido colocado
        GameManager.Instance.heartPlaced = true;

        // Eliminamos el ghost y salimos del modo colocación
        Destroy(ghost);
        isPlacingHeart = false;
    }

    /// <summary>
    /// Se encarga de decidir si el corazón se puede colocar en la celda seleccionada
    /// </summary>
    /// <param name="cell">Celda donde comprobar si se puede colocar el corazón</param>
    /// <returns>True si se puede colocar, false si no</returns>
    bool CanPlaceHeartAt(Vector3Int cell)
    {
        // Obtenemos todas las celdas que ocupa el corazón
        MultiCellPiece multi = ghost.GetComponent<MultiCellPiece>();

        // Recorremos todas las celdas y abortamos si alguna está fuera de límites o encima de otra pieza
        foreach (var offset in multi.occupiedOffsets)
        {
            Vector3Int target = cell + offset;

            if (!grid.IsInsideBounds(target))
                return false;

            if (grid.placedObjects.ContainsKey(target))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Cambia el color a todas las piezas del ghost del corazón según si está en una posición válida (verde) o no (rojo)
    /// </summary>
    /// <param name="valid"></param>
    void SetGhostColor(bool valid)
    {
        Color c = valid ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);

        // Aplicamos el color a todos los renders del ghost
        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
        {
            r.material.color = c;
        }
    }
}
