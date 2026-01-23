using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Gestiona la grid, la colocación de carreteras y salidas, la rotación de las mismas, la previsualización (ghost) y los límites del mapa
/// </summary>
public class GridSystem : MonoBehaviour
{
    public GameObject selectedObject;                                       // Apunta al prefab seleccionado de la lista
    public int selectedObjectIndex;                                         // Índice del prefab seleccionado
    public GameObject[] objectsToPlace;                                     // Vector de prefabs disponibles
    public float gridSize = 1f;                                             // Tamaño de las celdas del grid
    private GameObject ghostObject;                                         // Objeto fantasma que sigue al ratón, ghost
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();    // Conjunto de posiciones ocupadas

    private Quaternion currentRotation = Quaternion.identity;               // Rotación actual del ghost y de las piezas colocadas

    public Dictionary<Vector3, GameObject> placedObjects = new Dictionary<Vector3, GameObject>();   // Diccionario que guarda todas las piezas colocadas
    public Dictionary<Vector3Int, RoadOutput> outputs = new Dictionary<Vector3Int, RoadOutput>();   // Diccionario de salidas colocadas

    public Vector2Int minBounds = new Vector2Int(-16, -8);  // Límite del área mínima donde se pueden colocar piezas
    public Vector2Int maxBounds = new Vector2Int(16, 8);    // Límite del área máxima donde se pueden colocar piezas

    public Camera previewCamera;                            // Cámara utilizada para mostrar las previews
    public PreviewController previewController;             // Apunta al controlador de previews

    private void Start()
    {
        previewController = FindObjectOfType<PreviewController>();
        // Selecciona el primer objeto del vector y crea el ghost inicial
        selectedObject = objectsToPlace[selectedObjectIndex];
        CreateGhostObject();
    }

    private void Update()
    {
        // Actualiza la posición del ghost según el ratón
        UpdateGhostPosition();

        // Click izquierdo -> Colocar pieza
        if (Input.GetMouseButton(0))
        {
            PlaceObject();
        }

        // Click derecho -> Borrar pieza
        if (Input.GetMouseButton(1))
        {
            EraseObject();
        }

        // Pulsar R -> Rotar pieza
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateGhost();
        }

        // Pulsar Q -> Cambiar pieza (hacia atrás)
        // Pulsar E -> Cambiar pieza (hacia delante)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeSelectedGhost(false);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeSelectedGhost(true);
        }
    }

    /// <summary>
    /// Instancia el ghost en escena
    /// </summary>
    void CreateGhostObject()
    {
        // Lo instancia y desactiva su collider para evitar colisiones
        ghostObject = Instantiate(selectedObject);
        ghostObject.GetComponent<Collider>().enabled = false;

        // Obtiene todos sus renderers y los modifica
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Lo hace semitransparente
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            // Configura el material para el renderizado transparente
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_ScrBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    /// <summary>
    /// Actualiza la posición del ghost y lo coloca en la posición del ratón
    /// </summary>
    void UpdateGhostPosition()
    {
        // Crea un rayo desde la cámara hacia el ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Si el rayo golpea algo
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            // Obtiene el punto de impacto, lo ajusta al grid y mueve el ghost a la posición
            Vector3 point = hit.point;
            Vector3 snapped = SnapPosition(point);
            ghostObject.transform.position = snapped;

            // Cambia el color del fantasma: Rojo -> Celda ocupada, Azul -> Fuera de límites, Blanco -> Válido
            if (occupiedPositions.Contains(snapped))
            {
                SetGhostColor(Color.red);
            }
            else if (!IsInsideBounds(snapped))
            {
                SetGhostColor(Color.blue);
                return;
            }
            else
            {
                SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
            }
        }
    }

    /// <summary>
    /// Le cambia el color al ghost
    /// </summary>
    /// <param name="color">Color al que tenemos que cambiar al ghost</param>
    void SetGhostColor(Color color)
    {
        // Obtiene todos sus renderers y les cambia el color al proporcionado
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    /// <summary>
    /// Coloca la carretera en la grid
    /// </summary>
    void PlaceObject()
    {
        // Obtiene la posición ajustada al grid
        Vector3 placementPosition = SnapPosition(ghostObject.transform.position);

        // No coloca si está fuera de límites
        if (!IsInsideBounds(placementPosition)) return;
        
        // Si la celda está libre
        if (!occupiedPositions.Contains(placementPosition))
        {
            // Instancia la pieza
            GameObject obj = Instantiate(selectedObject, placementPosition, currentRotation);

            // Copia las conexiones rotadas del ghost a la pieza real
            RoadPiece ghostRoad = ghostObject.GetComponent<RoadPiece>();
            RoadPiece placedRoad = obj.GetComponent<RoadPiece>();
            
            if (ghostRoad != null && placedRoad != null) {
                placedRoad.connections = (RoadDirection[])ghostRoad.connections.Clone();
            }

            // Registra la pieza en el diccionario y en el hash
            occupiedPositions.Add(placementPosition);
            placedObjects.Add(placementPosition, obj);
        }
    }

    /// <summary>
    /// Rota el ghost 90º
    /// </summary>
    void RotateGhost()
    {
        // Rota 90º en el eje Y, y actualiza la preview y el ghost
        currentRotation *= Quaternion.Euler(0f, 90f, 0f);
        previewController.RotatePreview(currentRotation);
        ghostObject.transform.rotation = currentRotation;

        // Si no es carreteram, no hay conexiones que rotar
        RoadPiece road = ghostObject.GetComponent<RoadPiece>();
        if (road == null || road.connections == null) return;

        // Rota las conexiones lógicamente
        for (int i = 0; i < road.connections.Length; i++)
        {
            int dir = (int)road.connections[i];
            dir = (dir + 1) % 4; // 4 direcciones en total
            road.connections[i] = (RoadDirection)dir;
        }
    }

    /// <summary>
    /// Si hay una pieza en la celda bajo el ghost, la borra
    /// </summary>
    void EraseObject()
    {
        Vector3 position = SnapPosition(ghostObject.transform.position);
        // Si hay una pieza, la borra, y la elimina del hash y del diccionario
        if (placedObjects.ContainsKey(position))
        {
            Destroy(placedObjects[position]);
            placedObjects.Remove(position);
            occupiedPositions.Remove(position);
        }
    }

    /// <summary>
    /// Ajusta la posición al grid
    /// </summary>
    /// <param name="pos">Posición proporcionada para ajustar</param>
    /// <returns>La posición ajustada correctamente</returns>
    Vector3 SnapPosition(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            0f,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    /// <summary>
    /// Cambia el ghost por el siguiente o el anterior dentro de las piezas disponibles
    /// </summary>
    /// <param name="up">Si es true, lo hace en positivo, si es false, en negativo</param>
    void ChangeSelectedGhost (bool up)
    {
        // Cambia el índice de la selección según la tecla utilizada
        if (up)
        {
            selectedObjectIndex++;
            if (selectedObjectIndex >= objectsToPlace.Length)
            {
                selectedObjectIndex = 0;
            }
        }
        else
        {
            selectedObjectIndex--;
            if (selectedObjectIndex < 0)
            {
                selectedObjectIndex = objectsToPlace.Length - 1;
            }
        }

        // Introduce el prefab del objeto seleccionado dentro del GameObject correspondiente
        selectedObject = objectsToPlace[selectedObjectIndex];

        // Destruye el ghost anterior
        Destroy(ghostObject);
        
        // Crea uno nuevo
        CreateGhostObject();

        // Mantiene la rotación visual
        ghostObject.transform.rotation = currentRotation;

        // Y mantiene la rotación lógica (conexiones)
        RoadPiece road = ghostObject.GetComponent<RoadPiece>();
        if (road != null && road.connections != null)
        {
            int steps = ((int)(currentRotation.eulerAngles.y / 90f)) % 4;

            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < road.connections.Length; i++)
                {
                    int dir = (int)road.connections[i];
                    dir = (dir + 1) % 4;
                    road.connections[i] = (RoadDirection)dir;
                }
            }
        }

        // Cambia también la preview a mostrar según la tecla utilizada
        previewController.ChangePreview(selectedObjectIndex);
    }

    /// <summary>
    /// Comprueba si una posición se encuentra dentro del área permitida
    /// </summary>
    /// <param name="pos">Posición a comprobar</param>
    /// <returns>True si se encuentra dentro del área, false si no</returns>
    bool IsInsideBounds(Vector3 pos)
    {
        return pos.x >= minBounds.x &&
               pos.x <= maxBounds.x &&
               pos.z >= minBounds.y &&
               pos.z <= maxBounds.y;
    }
}