using UnityEngine;

public class HeartDrag3D : MonoBehaviour
{
    [SerializeField] GameObject heartPrefab;    // Referencia al prefab del corazón
    [SerializeField] bool dragging = false;     // Indica si el corazón está siendo arrastrado
    
    GameObject ghost;                           // Objeto para almacenar el ghost del corazón

    void Update()
    {
        // Si el corazón ya está colocado, abortamos
        if (GameManager.Instance.heartPlaced) return;

        // Si hacemos click encima del corazón del cajón, empezamos a arrastrar el corazón real
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverThis())
            {
                StartDragging();
            }
        }

        // Si estamos arrastrando el corazón y soltamos, dejamos de arrastrar
        if (dragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }

    /// <summary>
    /// Comprueba si el ratón se encuentra encima del corazón del cajón
    /// </summary>
    /// <returns>True si se encuentra encima, false si no</returns>
    bool IsMouseOverThis()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    /// <summary>
    /// Gestiona lo que ocurre al empezar a arrastrar el corazón
    /// </summary>
    void StartDragging()
    {
        // Activamos el estado de arrastrado
        dragging = true;

        // Ocultamos el corazón pequeño
        DespawnMiniHeart();

        // Creamos el ghost del corazón real
        ghost = Instantiate(heartPrefab);
        HeartPlacementController.Instance.BeginDragGhost(ghost);
    }

    /// <summary>
    /// Gestiona lo que ocurre cuando terminamos de arrastrar el corazón
    /// </summary>
    void StopDragging()
    {
        // Desactivamos el modo de arrastrado
        dragging = false;

        // Eliminamos el ghost del corazón real
        bool placed = HeartPlacementController.Instance.EndDragGhost();

        // Si no se ha colocado, volvemos a mostrar el corazón del cajón
        if (!placed)
        {
            SpawnMiniHeart();
        }
    }

    /// <summary>
    /// Oculta el corazón del cajón
    /// </summary>
    public void DespawnMiniHeart()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Muestra el corazón del cajón
    /// </summary>
    public void SpawnMiniHeart()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}