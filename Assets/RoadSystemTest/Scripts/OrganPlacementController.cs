using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrganPlacementController : MonoBehaviour
{
    public static OrganPlacementController Instance { get; private set; }

    [HideInInspector] public bool isPlacingOrgan = false;
       
    [SerializeField] OrganData[] organs;

    OrganData currentOrganData;
    GameObject ghost;
    GridManager grid;
    Camera cam;

    private Vector3Int organCell;
    private bool organInitialized = false;
    private float stickHoldTimer = 0f;
    private Vector2 previousStick = Vector2.zero;

    private const float stickInitialDelay = 0.25f;
    private const float stickRepeatRate = 0.12f;

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
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f)
            CursorManager.IsGamepadMode = false;

        if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f)
            CursorManager.IsGamepadMode = true;

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

        if (!CursorManager.IsGamepadMode)
        {
            Ray ray = cam.ScreenPointToRay(CursorManager.Position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3Int cell = grid.Snap(hit.point);
                ghost.transform.position = cell + new Vector3Int(0, 3, 0);

                bool valid = CanPlaceOrganAt(cell);
                SetGhostColor(valid);
            }

            return;
        }

        if (!organInitialized)
        {
            organCell = grid.ClampToBounds(Vector3Int.zero);
            organInitialized = true;
        }

        Vector2 move = Gamepad.current.leftStick.ReadValue();
        float absX = Mathf.Abs(move.x);
        float absY = Mathf.Abs(move.y);
        float threshold = 0.5f;

        bool justPressed =
            (absX > threshold && Mathf.Abs(previousStick.x) <= threshold) ||
            (absY > threshold && Mathf.Abs(previousStick.y) <= threshold);

        bool held = absX > threshold || absY > threshold;

        if (justPressed)
        {
            if (absX > absY)
            {
                if (move.x > 0) organCell.x++;
                else organCell.x--;
            }
            else
            {
                if (move.y > 0) organCell.z++;
                else organCell.z--;
            }

            stickHoldTimer = 0f;
        }
        else if (held)
        {
            stickHoldTimer += Time.deltaTime;

            if (stickHoldTimer > stickInitialDelay)
            {
                if ((stickHoldTimer - stickInitialDelay) % stickRepeatRate < Time.deltaTime)
                {
                    if (absX > absY)
                    {
                        if (move.x > 0) organCell.x++;
                        else organCell.x--;
                    }
                    else
                    {
                        if (move.y > 0) organCell.z++;
                        else organCell.z--;
                    }
                }
            }
        }
        else
        {
            stickHoldTimer = 0f;
        }

        previousStick = move;

        organCell = grid.ClampToBounds(organCell);

        ghost.transform.position = organCell + new Vector3Int(0, 3, 0);

        bool valid2 = CanPlaceOrganAt(organCell);
        SetGhostColor(valid2);
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