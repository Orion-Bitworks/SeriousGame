using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Se encarga de gestionar la colocación y eliminación de todas las tuberías en la grid, así como del sistema de undo/redo
/// </summary>
public class BuildController : MonoBehaviour
{
    public static BuildController Instance { get; private set; }    // Referencia Singleton

    [SerializeField] GameObject[] objectsToPlace;                   // Vector de prefabs disponibles
    [SerializeField] WorldSpaceButton leftButton;                   // Referencia al botón de cambiar de pieza (hacia atrás)
    [SerializeField] WorldSpaceButton rightButton;                  // Referencia al botón de cambiar de pieza (hacia delante)
    [SerializeField] WorldSpaceButton rotateButton;                 // Referencia al botón de rotar pieza
    [SerializeField] Button undoButton;                             // Referencia al botón de deshacer pieza
    [SerializeField] Button redoButton;                             // Referencia al botón de rehacer pieza
    
    [HideInInspector] public bool isUndoing;                        // Controla si el usuario está deshaciendo acciones
    [HideInInspector] public bool isRedoing;                        // Controla si el usuario está rehaciendo acciones
    [HideInInspector] public float undoHoldTimer = 0f;              // Temporizador para mantener el "deshacer"
    [HideInInspector] public float redoHoldTimer = 0f;              // Temporizador para mantener el "rehacer"
    
    int selectedIndex = 0;                  // Índice del prefab seleccionado

    GridManager grid;                       // Referencia al sistema de la grid
    GhostController ghost;                  // Referencia al sistema del ghost
    PreviewController previewController;    // Referencia al controlador de previews
    Controls controls;                      // Referencia al InputAction de juego

    bool isPlacing;                         // Controla si el usuario está colocando piezas
    bool isErasing;                         // Controla si el usuario está eliminando piezas

    bool isMouseInsideGrid = false;         // Controla si el ratón se encuentra dentro de la grid

    const float initialDelay = 0.3f;        // Delay antes de repetir
    const float repeatRate = 0.08f;         // Tiempo entre repeticiones

    Stack<BuildAction> undoStack = new Stack<BuildAction>();    // Pila con las posibles acciones a deshacer
    Stack<BuildAction> redoStack = new Stack<BuildAction>();    // Pila con las posibles acciones a rehacer

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    private void Start()
    {
        grid = GridManager.Instance;
        ghost = GhostController.Instance;
        previewController = PreviewController.Instance;

        // Creamos un objeto nada más empezar
        ghost.CreateGhost(objectsToPlace[selectedIndex]);

        // Mostramos la preview adecuada
        previewController.ChangePreview(selectedIndex);

        // Activamos el inputAction
        controls = new Controls();
        controls.Enable();

        // Suscribimos todos los inputActions a sus funciones correspondientes
        controls.InRoadGame.Place.started += OnPlaceStarted;
        controls.InRoadGame.Place.canceled += OnPlaceCanceled;
        controls.InRoadGame.Erase.started += OnEraseStarted;
        controls.InRoadGame.Erase.canceled += OnEraseCanceled;
        controls.InRoadGame.Rotate.performed += OnRotate;
        controls.InRoadGame.PrevPiece.performed += OnPrevPiece;
        controls.InRoadGame.NextPiece.performed += OnNextPiece;
        controls.InRoadGame.Undo.started += OnUndoStarted;
        controls.InRoadGame.Undo.canceled += OnUndoCanceled;
        controls.InRoadGame.Redo.started += OnRedoStarted;
        controls.InRoadGame.Redo.canceled += OnRedoCanceled;
    }

    private void OnDestroy()
    {
        // Desuscribimos todos los inputActions a sus funciones correspondientes
        if (controls == null) return;

        controls.InRoadGame.Place.started -= OnPlaceStarted;
        controls.InRoadGame.Place.canceled -= OnPlaceCanceled;
        controls.InRoadGame.Erase.started -= OnEraseStarted;
        controls.InRoadGame.Erase.canceled -= OnEraseCanceled;
        controls.InRoadGame.Rotate.performed -= OnRotate;
        controls.InRoadGame.PrevPiece.performed -= OnPrevPiece;
        controls.InRoadGame.NextPiece.performed -= OnNextPiece;
        controls.InRoadGame.Undo.started -= OnUndoStarted;
        controls.InRoadGame.Undo.canceled -= OnUndoCanceled;
        controls.InRoadGame.Redo.started -= OnRedoStarted;
        controls.InRoadGame.Redo.canceled -= OnRedoCanceled;

        controls.Disable();
    }

    // Lista de funciones para las distintas acciones de los inputActions
    private void OnPlaceStarted(InputAction.CallbackContext ctx) => isPlacing = true;
    private void OnPlaceCanceled(InputAction.CallbackContext ctx) => isPlacing = false;
    private void OnEraseStarted(InputAction.CallbackContext ctx) => isErasing = true;
    private void OnEraseCanceled(InputAction.CallbackContext ctx) => isErasing = false;
    private void OnRotate(InputAction.CallbackContext ctx) => RotateGhostAndPreview();
    private void OnPrevPiece(InputAction.CallbackContext ctx) => ChangeObject(false);
    private void OnNextPiece(InputAction.CallbackContext ctx) => ChangeObject(true);
    private void OnUndoStarted(InputAction.CallbackContext ctx)
    {
        isUndoing = true;
        undoHoldTimer = 0f;
    }
    private void OnUndoCanceled(InputAction.CallbackContext ctx) => isUndoing = false;
    private void OnRedoStarted(InputAction.CallbackContext ctx)
    {
        isRedoing = true;
        redoHoldTimer = 0f;
    }
    private void OnRedoCanceled(InputAction.CallbackContext ctx) => isRedoing = false;

    /// <summary>
    /// Rota tanto el ghost como la preview
    /// </summary>
    public void RotateGhostAndPreview()
    {
        // Si el sistema está en marcha, abortamos
        if (GameManager.Instance.isPlaying) return;

        ghost.RotateGhost();
        previewController.RotatePreview(ghost.currentRotation);
        StartCoroutine(PressUIButton(rotateButton));
    }

    private void Update()
    {
        // Actualizamos la posición del ghost y hacemos acciones según el input del usuario
        UpdateGhostMovement();

        // Mientras se mantiene el click derecho, colocamos objeto
        if (isPlacing)
            PlaceObject();

        // Mientras se mantiene el click izquierdo, borramos objeto
        if (isErasing)
            EraseObject();

        // Gestión del deshacer y rehacer
        HandleContinuousUndoRedo();
    }

    /// <summary>
    /// Gestiona el deshacer y rehacer cuando se mantienen las teclas, haciéndolo fluido
    /// </summary>
    void HandleContinuousUndoRedo()
    {
        // Si el sistema está en marcha, abortamos
        if (GameManager.Instance.isPlaying) return;

        if (isUndoing)
        {
            // Primer undo instantáneo, después del delay, repetir cada "repeatRate", activamos el botón visualmente
            SetButtonPressed(undoButton);
            if (undoHoldTimer == 0f)
                Undo();
            else if (undoHoldTimer > initialDelay)
            {
                if ((undoHoldTimer - initialDelay) % repeatRate < Time.deltaTime)
                    Undo();
            }
            undoHoldTimer += Time.deltaTime;
        }

        if (isRedoing)
        {
            // Primer redo instantáneo, después del delay, repetir cada "repeatRate", activamos el botón visualmente
            SetButtonPressed(redoButton);
            if (redoHoldTimer == 0f)
                Redo();
            else if (redoHoldTimer > initialDelay)
            {
                if ((redoHoldTimer - initialDelay) % repeatRate < Time.deltaTime)
                    Redo();
            }
            redoHoldTimer += Time.deltaTime;
        }

        // Reseteamos los temporizadores al soltar la tecla y ponemos su botón en el estado normal
        if (!isUndoing)
        {
            undoHoldTimer = 0f;
            SetButtonNormal(undoButton);
        }

        if (!isRedoing)
        {
            redoHoldTimer = 0f;
            SetButtonNormal(redoButton);
        }
    }

    /// <summary>
    /// Actualiza la posición del ghost y lo coloca en la posición del ratón
    /// </summary>
    void UpdateGhostMovement()
    {
        // Si el sistema está en marcha, abortamos y dejamos de mostrar el fantasma
        if (GameManager.Instance.isPlaying || OrganPlacementController.Instance.isPlacingOrgan)
        {
            ghost.ghostObject.SetActive(false);
            return;
        }

        // Crea un rayo desde la cámara hacia el ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int mask = LayerMask.GetMask("Grid");

        // Si el rayo golpea algo
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mask))
        {
            // Obtiene el punto de impacto, lo ajusta al grid y le manda al ghost la información relevante
            Vector3Int snapped = grid.Snap(hit.point);
            snapped = grid.ClampToBounds(snapped);

            bool inside = grid.IsInsideBounds(snapped);
            bool occupied = grid.placedObjects.ContainsKey(snapped);

            if (hit.collider.GetComponentInParent<MultiCellPiece>() != null)
                occupied = true;

            // Guardamos si el ratón está dentro de la grid
            isMouseInsideGrid = inside;

            ghost.UpdateGhostPosition(snapped, occupied, inside);
        }
        else
        {
            // Si no golpea nada, el ratón está fuera de la grid
            isMouseInsideGrid = false;
        }
        // Mostramos o no el ghost dependiendo de si estamos con el ratón dentro de la grid o no
        ghost.ghostObject.SetActive(isMouseInsideGrid);
    }

    /// <summary>
    /// Coloca la carretera en la grid
    /// </summary>
    void PlaceObject()
    {
        // Si el sistema está en marcha, abortamos
        if (GameManager.Instance.isPlaying) return;

        // Si el ratón está fuera de la grid, no colocamos pieza
        if (!isMouseInsideGrid) return;

        // Obtiene la posición ajustada al grid
        Vector3Int cell = Vector3Int.RoundToInt(ghost.ghostObject.transform.position);

        // No coloca si está fuera de límites o si la celda está ocupada
        if (!grid.IsInsideBounds(cell)) return;

        // Si ya hay una pieza en esa celda, comprobamos si es la misma
        if (grid.placedObjects.ContainsKey(cell))
        {
            GameObject existing = grid.placedObjects[cell];

            // Comprobamos si es el mismo prefab
            PlacedPiece placed = existing.GetComponent<PlacedPiece>();
            if (placed != null && placed.originalPrefab == objectsToPlace[selectedIndex])
            {
                // Comprobamos si tienen la misma rotación, y si coincide, abortamos
                if (existing.transform.rotation == ghost.currentRotation)
                    return;
            }

            // Si no es la misma pieza, entonces sí borramos
            EraseObject();
        }

        //if (grid.placedObjects.ContainsKey(cell)) EraseObject();

        // Bloquea colocación encima de órganos
        if (Physics.Raycast(cell + Vector3.up * 5f, Vector3.down, out RaycastHit h, 10f))
        {
            if (h.collider.GetComponentInParent<MultiCellPiece>() != null)
                return;
        }

        // Instancia la pieza
        GameObject obj = Instantiate(objectsToPlace[selectedIndex], cell, ghost.currentRotation);

        obj.AddComponent<PlacedPiece>().originalPrefab = objectsToPlace[selectedIndex];

        // Copia las conexiones rotadas del ghost a la pieza real
        RoadPiece ghostRoad = ghost.ghostObject.GetComponent<RoadPiece>();
        RoadPiece placedRoad = obj.GetComponent<RoadPiece>();

        if (ghostRoad != null && placedRoad != null)
            placedRoad.connections = (RoadDirection[])ghostRoad.connections.Clone();

        // Registra la pieza en el diccionario
        grid.placedObjects.Add(cell, obj);

        // Registramos acción para rehacer
        undoStack.Push(new BuildAction(
            BuildActionType.Place,
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
        // Si el sistema está en marcha, abortamos
        if (GameManager.Instance.isPlaying) return;

        // Si el ratón está fuera de la grid, no borramos pieza
        if (!isMouseInsideGrid) return;

        Vector3Int cell = Vector3Int.RoundToInt(ghost.ghostObject.transform.position);
        // Si no hay pieza bajo el ghost, aborta
        if (!grid.placedObjects.ContainsKey(cell))
            return;

        // Bloquea borrar piezas de órganos
        if (Physics.Raycast(cell + Vector3.up * 5f, Vector3.down, out RaycastHit h, 10f))
        {
            if (h.collider.GetComponentInParent<MultiCellPiece>() != null)
                return;
        }

        // Obtenemos el objeto y su RoadPiece
        GameObject obj = grid.placedObjects[cell];
        PlacedPiece placed = obj.GetComponent<PlacedPiece>();
        RoadPiece piece = obj.GetComponent<RoadPiece>();

        // Registramos acción para rehacer
        undoStack.Push(new BuildAction(
            BuildActionType.Erase,
            cell,
            placed.originalPrefab,
            obj.transform.rotation,
            piece != null ? piece.connections : null
        ));

        redoStack.Clear();

        // La borramos, y eliminamos del diccionario
        Destroy(obj);
        grid.placedObjects.Remove(cell);
    }

    /// <summary>
    /// Cambia el ghost por el siguiente o el anterior dentro de las piezas disponibles
    /// </summary>
    /// <param name="next">Si es true, lo hace en positivo, si es false, en negativo</param>
    public void ChangeObject(bool next)
    {
        // Si el sistema está en marcha, abortamos
        if (GameManager.Instance.isPlaying) return;

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
        if (next)
            StartCoroutine(PressUIButton(rightButton));
        else
            StartCoroutine(PressUIButton(leftButton));
    }

    /// <summary>
    /// Utilizado para la acción deshacer
    /// </summary>
    public void Undo()
    {
        // Si la pila de deshacer está vacía, abortamos
        if (undoStack.Count == 0)
            return;

        // Cogemos la última acción
        BuildAction action = undoStack.Pop();

        switch (action.type)
        {
            case BuildActionType.Place:
                // Borramos la pieza actual y la borramos del diccionario
                if (grid.placedObjects.ContainsKey(action.cell))
                {
                    Destroy(grid.placedObjects[action.cell]);
                    grid.placedObjects.Remove(action.cell);
                }
                break;
            case BuildActionType.Erase:
                // Volvemos a colocar la pieza borrada
                GameObject obj = Instantiate(action.prefab, action.cell, action.rotation);

                obj.AddComponent<PlacedPiece>().originalPrefab = objectsToPlace[selectedIndex];

                RoadPiece piece = GetComponent<RoadPiece>();
                if (piece != null && action.connections != null)
                    piece.connections = (RoadDirection[])action.connections.Clone();

                grid.placedObjects.Add(action.cell, obj);
                break;
            case BuildActionType.OrganPlace:
                // Eliminar el órgano y spawnear objeto del cajón
                if (grid.placedObjects.ContainsKey(action.cell))
                {
                    var organObj = grid.placedObjects[action.cell];
                    Destroy(organObj);
                    grid.placedObjects.Remove(action.cell);
                    FindAnyObjectByType<OrganDrag3D>().SpawnMiniOrgan();
                }

                // Desregistrar tuberías internas
                OrganPlacementController.Instance.UnregisterOrganInternalPipes();

                if (action.organData != null)
                    action.organData.isPlaced = false;
                break;
        }

        // Guardamos en la pila de rehacer
        redoStack.Push(action);
    }

    /// <summary>
    /// Utilizado para la acción rehacer
    /// </summary>
    public void Redo()
    {
        // Si la pila de rehacer está vacía, abortamos
        if (redoStack.Count == 0)
            return;

        // Cogemos la última acción
        BuildAction action = redoStack.Pop();

        switch (action.type)
        {
            case BuildActionType.Place:
                // Volvemos a colocar la pieza
                GameObject obj = Instantiate(action.prefab, action.cell, action.rotation);

                obj.AddComponent<PlacedPiece>().originalPrefab = objectsToPlace[selectedIndex];

                RoadPiece piece = obj.GetComponent<RoadPiece>();
                if (piece != null && action.connections != null)
                    piece.connections = (RoadDirection[])action.connections.Clone();

                grid.placedObjects.Add(action.cell, obj);
                break;
            case BuildActionType.Erase:
                // Recolocamos la pieza borrada
                if (grid.placedObjects.ContainsKey(action.cell))
                {
                    Destroy(grid.placedObjects[action.cell]);
                    grid.placedObjects.Remove(action.cell);
                }
                break;
            case BuildActionType.OrganPlace:
                // Volver a colocar el órgano, registrar tuberías y despawnear objeto del cajón
                GameObject organ = Instantiate(action.prefab, action.cell, action.rotation);

                var reg = organ.GetComponent<InternalPipeRegister>();
                reg.Register(grid);

                grid.placedObjects[action.cell] = organ;
                FindAnyObjectByType<OrganDrag3D>().DespawnMiniOrgan();

                if (action.organData != null)
                    action.organData.isPlaced = true;
                break;
        }

        // Guardamos en la pila de deshacer
        undoStack.Push(action);
    }

    /// <summary>
    /// Simula visualmente una pulsación de botón en UI
    /// </summary>
    /// <param name="button">El botón a modificar visualmente</param>
    IEnumerator PressUIButton(WorldSpaceButton button)
    {
        // Simula pulsar el botón visualmente
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);

        yield return new WaitForSeconds(0.15f);

        // Simula soltar el botón visualmente
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
    }

    /// <summary>
    /// Le cambia el color al botón para que parezca que está siendo pulsado
    /// ¡IMPORTANTE, SE UTILIZA PARA UNDO/REDO DEBIDO A QUE SE PUEDEN MANTENER!
    /// </summary>
    /// <param name="button">Botón a modificar visualmente</param>
    void SetButtonPressed(Button button)
    {
        if (button == null) return;

        var colors = button.colors;
        button.targetGraphic.color = colors.pressedColor;
    }

    /// <summary>
    /// Le cambia el color al botón para que parezca que ya no se está pulsando
    /// ¡IMPORTANTE, SE UTILIZA PARA UNDO/REDO DEBIDO A QUE SE PUEDEN MANTENER!
    /// </summary>
    /// <param name="button">Botón a modificar visualmente</param>
    void SetButtonNormal(Button button)
    {
        if (button == null) return;

        var colors = button.colors;
        button.targetGraphic.color = colors.normalColor;
    }

    /// <summary>
    /// Registra el órgano en la pila de deshacer
    /// </summary>
    /// <param name="cell">Celda donde se ha colocado</param>
    /// <param name="prefab">Prefab del órgano</param>
    /// <param name="rotation">Rotación del órgano</param>
    public void RegisterOrganPlaced(Vector3Int cell, GameObject prefab, Quaternion rotation, OrganData organData)
    {
        undoStack.Push(new BuildAction(
            BuildActionType.OrganPlace,
            cell,
            prefab,
            rotation,
            organData
        ));

        redoStack.Clear();
    }
}