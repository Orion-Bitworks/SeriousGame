using UnityEngine;
using UnityEngine.InputSystem;

public class OrganDrag3D : MonoBehaviour
{
    [Header("Organ Settings")]
    [SerializeField] public OrganData organData;      // ScriptableObject con info del órgano

    bool dragging = false;
    GameObject ghost;

    public Controls controls;

    private void Start()
    {
        controls = new Controls();
        controls.Enable();
    }

    void Update()
    {
        // Si el órgano ya está colocado, no se puede volver a arrastrar
        if (organData.isPlaced) return;

        // Si estamos en el nivel sin órganos, abortamos
        if (GameManager.Instance.currentLevel == LevelID.Pipe) return;

        if (!CursorManager.IsGamepadMode)
        {
            if (!dragging && Mouse.current.leftButton.wasPressedThisFrame && IsMouseOverThis())
                StartDragging();

            if (dragging && Mouse.current.leftButton.wasReleasedThisFrame)
                StopDragging();

            return;
        }

        // Iniciar arrastre
        if (controls.InRoadGame.Place.triggered && IsMouseOverThis())
            StartDragging();

        // Soltar órgano
        if (dragging && controls.InRoadGame.Place.triggered)
            StopDragging();
    }

    bool IsMouseOverThis()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    void StartDragging()
    {
        dragging = true;

        CursorManager.IsGamepadMode = false;

        // Ocultar mini-órgano del cajón
        DespawnMiniOrgan();

        // Crear ghost
        ghost = Instantiate(organData.prefab);

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.GrabOrgan);

        OrganPlacementController.Instance.BeginPlacingOrgan(organData, ghost);
    }

    void StopDragging()
    {
        dragging = false;

        bool placed = OrganPlacementController.Instance.EndPlacingOrgan();

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.DropOrgan);

        if (!placed)
            SpawnMiniOrgan(); // Si no se colocó, volver a mostrar el mini-órgano
    }

    public void DespawnMiniOrgan()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SpawnMiniOrgan()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void StartDraggingFromGamepad()
    {
        if (organData.isPlaced) return;

        dragging = true;

        DespawnMiniOrgan();

        ghost = Instantiate(organData.prefab);

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.GrabOrgan);

        OrganPlacementController.Instance.BeginPlacingOrgan(organData, ghost);
    }
}
