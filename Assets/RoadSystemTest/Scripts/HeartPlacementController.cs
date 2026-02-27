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

    // Actualizamos la posición del ghost en cada frame
    private void Update()
    {
        UpdateGhostPosition();
    }

    /// <summary>
    /// Actualiza la posición del ghost respecto a la posición del ratón en pantalla
    /// </summary>
    void UpdateGhostPosition()
    {
        if (!isPlacingHeart) return;

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
    /// Empezamos a arrastrar el ghost del corazón
    /// </summary>
    /// <param name="ghostObj">Objeto del corazón fantasma</param>
    public void BeginDragGhost(GameObject ghostObj)
    {
        // Si el corazón ya está colocado, abortamos
        if (GameManager.Instance.heartPlaced) return;

        ghost = ghostObj;
        isPlacingHeart = true;
    }

    /// <summary>
    /// Dejamos de arrastrar el ghost del corazón
    /// </summary>
    /// <returns></returns>
    public bool EndDragGhost()
    {
        // Si ya no estamos colocando, devolvemos false
        if (!isPlacingHeart) return false;

        // Desactivamos el modo colocación
        isPlacingHeart = false;

        Vector3Int cell = Vector3Int.RoundToInt(ghost.transform.position);

        // Comprobamos si se puede colocar el corazón, y si se puede lo colocamos, eliminando el ghost y devolviendo true
        if (CanPlaceHeartAt(cell))
        {
            PlaceHeartAt(cell);
            Destroy(ghost);
            ghost = null;
            return true;
        }

        // Si no se puede colocar, eliminamos el ghost y devolvemos false
        Destroy(ghost);
        ghost = null;
        return false;
    }

    /// <summary>
    /// Gestiona la colocación del corazón en la grid
    /// </summary>
    /// <param name="cell">Celda de la grid donde colocar el corazón</param>
    void PlaceHeartAt(Vector3Int cell)
    {
        // Instanciamos el corazón
        GameObject heart = Instantiate(heartPrefab, cell, Quaternion.identity);

        // Registramos sus tuberías internas y el propio objeto del corazón
        var reg = heart.GetComponent<InternalPipeRegister>();
        reg.Register(GridManager.Instance);
        grid.placedObjects[cell] = heart;

        // Marcamos el corazón como colocado y lo registramos en la pila de deshacer
        GameManager.Instance.heartPlaced = true;
        BuildController.Instance.RegisterHeartPlaced(cell, heartPrefab, heart.transform.rotation);
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

    /// <summary>
    /// Se encarga de mandar a desregistrar las tuberías del corazón
    /// </summary>
    public void UnregisterHeartInternalPipes()
    {
        var reg = FindObjectOfType<InternalPipeRegister>();
        if (reg != null)
            reg.Unregister(GridManager.Instance);
    }
}
