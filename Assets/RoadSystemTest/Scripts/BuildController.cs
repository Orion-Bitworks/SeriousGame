using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    public GameObject[] objectsToPlace;             // Vector de prefabs disponibles
    public int selectedIndex = 0;                   // Índice del prefab seleccionado

    private GridManager grid;                       // Referencia al sistema de la grid
    private GhostController ghost;                  // Referencia al sistema del ghost
    private PreviewController previewController;    // Referencia al controlador de previews

    private Stack<BuildAction> undoStack = new Stack<BuildAction>();    // Pila con las posibles acciones a deshacer
    private Stack<BuildAction> redoStack = new Stack<BuildAction>();    // Pila con las posibles acciones a rehacer

    private void Start()
    {
        grid = GridManager.Instance;
        ghost = GhostController.Instance;
        previewController = PreviewController.Instance;

        // Creamos un objeto nada más empezar
        ghost.CreateGhost(objectsToPlace[selectedIndex]);

        // Mostramos la preview adecuada
        previewController.ChangePreview(selectedIndex);
    }

    private void Update()
    {
        // Actualizamos la posición del ghost y hacemos acciones según el input del usuario
        UpdateGhostMovement();
        HandleInput();
    }

    /// <summary>
    /// Actualiza la posición del ghost y lo coloca en la posición del ratón
    /// </summary>
    void UpdateGhostMovement()
    {
        // Crea un rayo desde la cámara hacia el ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Si el rayo golpea algo
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Obtiene el punto de impacto, lo ajusta al grid y le manda al ghost la información relevante
            Vector3Int snapped = grid.Snap(hit.point);
            snapped = grid.ClampToBounds(snapped);

            bool inside = grid.IsInsideBounds(snapped);
            bool occupied = grid.placedObjects.ContainsKey(snapped);

            ghost.UpdateGhostPosition(snapped, occupied, inside);
        }
    }

    /// <summary>
    /// Gestiona los inputs de usuario (TEMPORAL)
    /// </summary>
    void HandleInput()
    {
        // Click izquierdo -> Colocar pieza
        if (Input.GetMouseButton(0))
            PlaceObject();

        // Click derecho -> Borrar pieza
        if (Input.GetMouseButton(1))
            EraseObject();

        // Pulsar R -> Rotar pieza y rotar preview
        if (Input.GetKeyDown(KeyCode.R))
        {
            ghost.RotateGhost();
            previewController.RotatePreview(ghost.currentRotation);
        }

        // Pulsar Q -> Cambiar pieza (hacia atrás)
        // Pulsar E -> Cambiar pieza (hacia delante)
        if (Input.GetKeyDown(KeyCode.Q))
            ChangeObject(false);
        else if (Input.GetKeyDown(KeyCode.E))
            ChangeObject(true);

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
            Undo();

        if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
            Redo();
    }

    /// <summary>
    /// Coloca la carretera en la grid
    /// </summary>
    void PlaceObject()
    {
        // Obtiene la posición ajustada al grid
        Vector3Int cell = Vector3Int.RoundToInt(ghost.ghostObject.transform.position);

        // No coloca si está fuera de límites o si la celda está ocupada
        if (!grid.IsInsideBounds(cell)) return;
        if (grid.placedObjects.ContainsKey(cell)) return;

        // Instancia la pieza
        GameObject obj = Instantiate(objectsToPlace[selectedIndex], cell, ghost.currentRotation);

        // Copia las conexiones rotadas del ghost a la pieza real
        RoadPiece ghostRoad = ghost.ghostObject.GetComponent<RoadPiece>();
        RoadPiece placedRoad = obj.GetComponent<RoadPiece>();

        if (ghostRoad != null && placedRoad != null)
            placedRoad.connections = (RoadDirection[])ghostRoad.connections.Clone();

        // Registra la pieza en el diccionario
        grid.placedObjects.Add(cell, obj);

        // Registramos acción para rehacer
        undoStack.Push(new BuildAction(
            cell,
            objectsToPlace[selectedIndex],
            ghost.currentRotation,
            placedRoad != null ? placedRoad.connections : null
        )); 
        
        // Limpiamos pila de rehacer
        redoStack.Clear();
    }

    /// <summary>
    /// Si hay una pieza en la celda bajo el ghost, la borra
    /// </summary>
    void EraseObject()
    {
        Vector3Int cell = Vector3Int.RoundToInt(ghost.ghostObject.transform.position);
        // Si hay una pieza, la borra, y la elimina del diccionario
        if (grid.placedObjects.ContainsKey(cell))
        {
            Destroy(grid.placedObjects[cell]);
            grid.placedObjects.Remove(cell);
        }
    }

    /// <summary>
    /// Cambia el ghost por el siguiente o el anterior dentro de las piezas disponibles
    /// </summary>
    /// <param name="next">Si es true, lo hace en positivo, si es false, en negativo</param>
    void ChangeObject(bool next)
    {
        // Cambia el índice de la selección según la tecla utilizada
        selectedIndex += next ? 1 : -1;
        if (selectedIndex >= objectsToPlace.Length) selectedIndex = 0;
        if (selectedIndex < 0) selectedIndex = objectsToPlace.Length - 1;

        // Crea el nuevo ghost con el prefab seleccionado, con su rotación actual
        ghost.CreateGhost(objectsToPlace[selectedIndex]);
        ghost.ghostObject.transform.rotation = ghost.currentRotation;

        // Y manteniendo la lógica de conexiones
        RoadPiece road = ghost.ghostObject.GetComponent<RoadPiece>();
        if (road != null && road.connections != null)
        {
            int steps = ((int)(ghost.currentRotation.eulerAngles.y / 90f)) % 4;

            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < road.connections.Length; i++)
                {
                    road.connections[i] = DirectionUtils.Rotate90(road.connections[i]);
                }
            }
        }
        // Cambia también la preview a mostrar según la tecla utilizada
        previewController.ChangePreview(selectedIndex);
    }

    /// <summary>
    /// Utilizado para la acción deshacer
    /// </summary>
    public void Undo()
    {
        if (undoStack.Count == 0)
            return;

        // Cogemos la última acción
        BuildAction action = undoStack.Pop();

        // Borramos la pieza actual y la borramos del diccionario
        if (grid.placedObjects.ContainsKey(action.cell))
        {
            Destroy(grid.placedObjects[action.cell]);
            grid.placedObjects.Remove(action.cell);
        }

        // Guardamos en la pila de rehacer
        redoStack.Push(action);
    }

    /// <summary>
    /// Utilizado para la acción rehacer
    /// </summary>
    public void Redo()
    {
        if (redoStack.Count == 0)
            return;

        // Cogemos la última acción
        BuildAction action = redoStack.Pop();

        // Volvemos a colocar la pieza
        GameObject obj = Instantiate(action.prefab, action.cell, action.rotation);

        RoadPiece piece = obj.GetComponent<RoadPiece>();
        if (piece != null && action.connections != null)
            piece.connections = (RoadDirection[])action.connections.Clone();

        grid.placedObjects.Add(action.cell, obj);

        // Guardamos en la pila de deshacer
        undoStack.Push(action);
    }

}
