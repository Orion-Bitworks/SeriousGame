using Unity.VisualScripting;
using UnityEngine;

public class OrganPlacementController : MonoBehaviour
{
    public static OrganPlacementController Instance { get; private set; }

    [HideInInspector] public bool isPlacingOrgan = false;
       
    [SerializeField] OrganData[] organs;

    OrganData currentOrganData;
    GameObject ghost;
    GridManager grid;
    Camera cam;

    private void Awake()
    {
        Instance = this;

        foreach(OrganData organ in organs)
        {
            organ.isPlaced = false;
        }
    }

    private void Start()
    {
        cam = Camera.main;
        grid = GridManager.Instance;
    }

    private void Update()
    {
        UpdateGhostPosition();
    }

    public void BeginPlacingOrgan(OrganData organData, GameObject ghostObj)
    {
        currentOrganData = organData;
        ghost = ghostObj;
        isPlacingOrgan = true;
    }

    public bool EndPlacingOrgan()
    {
        if (!isPlacingOrgan) return false;

        isPlacingOrgan = false;

        Vector3Int cell = Vector3Int.RoundToInt(ghost.transform.position) - new Vector3Int(0, 3, 0);

        if (CanPlaceOrganAt(cell))
        {
            PlaceOrganAt(cell);
            Destroy(ghost);
            ghost = null;
            return true;
        }

        Destroy(ghost);
        ghost = null;
        return false;
    }

    void UpdateGhostPosition()
    {
        if (!isPlacingOrgan) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int cell = grid.Snap(hit.point);
            ghost.transform.position = cell + new Vector3Int(0, 3, 0);

            bool valid = CanPlaceOrganAt(cell);
            SetGhostColor(valid);
        }
    }

    bool CanPlaceOrganAt(Vector3Int cell)
    {
        MultiCellPiece multi = ghost.GetComponent<MultiCellPiece>();

        foreach (var offset in multi.occupiedOffsets)
        {
            Vector3Int target = cell + offset;

            if (!grid.IsOrganInsideBounds(target))
                return false;

            if (grid.placedObjects.ContainsKey(target))
                return false;
        }

        return true;
    }

    void PlaceOrganAt(Vector3Int cell)
    {
        GameObject organ = Instantiate(currentOrganData.prefab, cell, Quaternion.identity);

        var reg = organ.GetComponent<InternalPipeRegister>();
        reg.Register(grid);
        grid.placedObjects[cell] = organ;

        currentOrganData.isPlaced = true;
        GameManager.Instance.NotifyOrganPlaced(currentOrganData, organ.transform.position);

        BuildController.Instance.RegisterOrganPlaced(cell, currentOrganData.prefab, organ.transform.rotation, currentOrganData);
    }

    void SetGhostColor(bool valid)
    {
        Color c = valid ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);

        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
            r.material.color = c;
    }

    public void UnregisterOrganInternalPipes()
    {
        var reg = FindObjectOfType<InternalPipeRegister>();
        if (reg != null)
            reg.Unregister(grid);

        if (currentOrganData != null)
            currentOrganData.isPlaced = false;
    }
}