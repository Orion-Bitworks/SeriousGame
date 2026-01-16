using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject selectedObject;
    public int selectedObjectIndex;
    public GameObject[] objectsToPlace;
    public float gridSize = 1f;
    private GameObject ghostObject;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private Quaternion currentRotation = Quaternion.identity;

    private Dictionary<Vector3, GameObject> placedObjects = new Dictionary<Vector3, GameObject>();

    public Vector2Int minBounds = new Vector2Int(-16, -8);
    public Vector2Int maxBounds = new Vector2Int(16, 8);

    private void Start()
    {
        selectedObject = objectsToPlace[selectedObjectIndex];
        CreateGhostObject();
    }

    private void Update()
    {
        UpdateGhostPosition();

        if (Input.GetMouseButton(0))
        {
            PlaceObject();
        }

        if (Input.GetMouseButton(1))
        {
            EraseObject();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateGhost();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeSelectedGhost(false);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeSelectedGhost(true);
        }
    }

    void CreateGhostObject()
    {
        ghostObject = Instantiate(selectedObject);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
    
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

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

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snapped = SnapPosition(point);

            ghostObject.transform.position = snapped;

            if (occupiedPositions.Contains(snapped))
            {
                SetGhostColor(Color.red);
            }
            else if (!IsInsideBounds(snapped))
            {
                SetGhostColor(Color.blue); // fuera del área
                return;
            }
            else
            {
                SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
            }
        }
    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    void PlaceObject()
    {
        Vector3 placementPosition = SnapPosition(ghostObject.transform.position);

        if (!IsInsideBounds(placementPosition)) return;

        if (!occupiedPositions.Contains(placementPosition))
        {
            GameObject obj = Instantiate(selectedObject, placementPosition, currentRotation);

            occupiedPositions.Add(placementPosition);
            placedObjects.Add(placementPosition, obj);
        }
    }

    void RotateGhost()
    {
        currentRotation *= Quaternion.Euler(0f, 90f, 0f);
        ghostObject.transform.rotation = currentRotation;
        for (int i = 0; i < ghostObject.GetComponent<RoadPiece>().connections.Length; i++)
        {
            if ((int)ghostObject.GetComponent<RoadPiece>().connections[i] > ghostObject.GetComponent<RoadPiece>().connections.Length)
            {
                ghostObject.GetComponent<RoadPiece>().connections[i] = 0;
            }
            else ghostObject.GetComponent<RoadPiece>().connections[i]++;
        }
    }

    void EraseObject()
    {
        Vector3 position = SnapPosition(ghostObject.transform.position);

        if (placedObjects.ContainsKey(position))
        {
            Destroy(placedObjects[position]);
            placedObjects.Remove(position);
            occupiedPositions.Remove(position);
        }
    }
    Vector3 SnapPosition(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            0f,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    void ChangeSelectedGhost (bool up)
    {
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

        selectedObject = objectsToPlace[selectedObjectIndex];

        // Destruir ghost anterior
        Destroy(ghostObject);
        
        // Crear uno nuevo
        CreateGhostObject();
        
        // Mantener la rotación actual
        ghostObject.transform.rotation = currentRotation;
    }

    bool IsInsideBounds(Vector3 pos)
    {
        return pos.x >= minBounds.x &&
               pos.x <= maxBounds.x &&
               pos.z >= minBounds.y &&
               pos.z <= maxBounds.y;
    }
}
