using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropMinigame2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static DragAndDropMinigame2 currentlySelected = null;

    Vector3 offset;
    public string destinationTag = "DropArea";
    public bool placed = false;
    public Transform CurrentDropArea; //No rellenar
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public SelectObject selectObj;

    public Minigame1 minigame1Instance;
    public Minigame2 minigame2Instance;
    public string valveType;

    public bool locked = false;

    public Transform visualPivot;

    [Header("Colliders")]
    public Collider dragCollider;   // NO trigger, para el click
    public Collider tipTrigger;     // Trigger en la punta

    private bool hasDragged = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        selectObj = GetComponent<SelectObject>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (locked) return;
        if (DialogManager.IsDialogActive) return;

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Select);

        minigame2Instance.movimientos++;

        hasDragged = false;

        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;

            placed = false;
            CurrentDropArea = null;
        }

        if (currentlySelected != null && currentlySelected != this)
            currentlySelected.selectObj?.Deselect();

        selectObj.Select();
        currentlySelected = this;

        if (ObjectSelector.currentlySelected != null && ObjectSelector.currentlySelected.gameObject != gameObject)
            ObjectSelector.currentlySelected.Deselect();

        ObjectSelector.currentlySelected = selectObj;
        selectObj.Select();

        offset = transform.position - MouseWorldPosition();
    }

    /*void OnMouseDown()
    {
        if (locked) return;
        if (DialogManager.IsDialogActive) return;

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Select);
        minigame2Instance.movimientos++;

        hasDragged = false;

        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;

            placed = false;
            CurrentDropArea = null;
        }

        if (currentlySelected != null && currentlySelected != this)
            currentlySelected.selectObj?.Deselect();

        selectObj.Select();
        currentlySelected = this;

        if (ObjectSelector.currentlySelected != null && ObjectSelector.currentlySelected.gameObject != gameObject)
            ObjectSelector.currentlySelected.Deselect();

        ObjectSelector.currentlySelected = selectObj;
        selectObj.Select();

        offset = transform.position - MouseWorldPosition();

        // Solo desactivamos el collider de drag, NO el trigger
        //if (dragCollider != null)
        //    dragCollider.enabled = false;
    }*/

    public void OnDrag(PointerEventData eventData)
    {
        if (locked) return;
        if (DialogManager.IsDialogActive) return;

        Vector3 newPos = MouseWorldPosition() + offset;

        hasDragged = true;

        // Si estaba colocada y empieza a arrastrarse → liberar DropArea
        if (hasDragged && placed)
        {
            if (CurrentDropArea != null)
            {
                DropArea drop = CurrentDropArea.GetComponent<DropArea>();
                if (drop != null) drop.occupied = false;
            }

            placed = false;
            CurrentDropArea = null;

            if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                minigame2Instance.objectsRemaining();
        }

        transform.position = newPos;
    }

    /*void OnMouseDrag()
    {
        if (locked) return;

        Vector3 newPos = MouseWorldPosition() + offset;

        hasDragged = true;

        // Si estaba colocada y empieza a arrastrarse → liberar DropArea
        if (hasDragged && placed)
        {
            if (CurrentDropArea != null)
            {
                DropArea drop = CurrentDropArea.GetComponent<DropArea>();
                if (drop != null) drop.occupied = false;
            }

            placed = false;
            CurrentDropArea = null;

            if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                minigame2Instance.objectsRemaining();
        }

        transform.position = newPos;
    }*/

    public void OnPointerUp(PointerEventData eventData)
    {
        if (locked) return;
        if (DialogManager.IsDialogActive) return;

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Place);

        if ((!placed || hasDragged) && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null && !drop.occupied)
            {
                Debug.Log("Buscando SnapPivot en: " + CurrentDropArea.name);
                Transform snapPivot = CurrentDropArea.Find("SnapPivot");
                Debug.Log("SnapPivot encontrado: " + snapPivot);

                if (snapPivot != null)
                {
                    // Snap EXACTO al pivote central
                    Debug.Log("Snap pivot detectado");
                    transform.position = snapPivot.position;
                    ParticleManager.instance.SpawnParticles("PlacementDust", transform.position, Quaternion.identity);
                    //transform.rotation = drop.requiredRotation;

                    drop.occupied = true;
                    placed = true;

                    if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                        minigame2Instance.objectsRemaining();

                }
            }
            else
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
                placed = false;
                CurrentDropArea = null;
            }
        }
        else
        {
            // Si no se arrastró pero estaba colocada, NO volver al inicio
            if (hasDragged && !placed)
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
        }
    }

    /*void OnMouseUp()
    {
        if (locked) return;
        if (DialogManager.IsDialogActive) return;

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Place);

        if ((!placed || hasDragged) && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null && !drop.occupied)
            {
                Debug.Log("Buscando SnapPivot en: " + CurrentDropArea.name);
                Transform snapPivot = CurrentDropArea.Find("SnapPivot");
                Debug.Log("SnapPivot encontrado: " + snapPivot);

                if (snapPivot != null)
                {
                    // Snap EXACTO al pivote central
                    Debug.Log("Snap pivot detectado");
                    transform.position = snapPivot.position;
                    ParticleManager.instance.SpawnParticles("PlacementDust", transform.position, Quaternion.identity);
                    //transform.rotation = drop.requiredRotation;

                    drop.occupied = true;
                    placed = true;

                    if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                        minigame2Instance.objectsRemaining();
                    
                }
            }
        }
        else
        {
            // Si no se arrastró pero estaba colocada, NO volver al inicio
            if (hasDragged && !placed)
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
        }

        // Volvemos a activar el collider de drag
        //if (dragCollider != null)
        //    dragCollider.enabled = true;
    }*/

    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = (Vector3)CursorManager.Position; //Coge la posición del ratón en pantalla
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; //Ajusta z usando la distancia del objeto a la cámara
        return Camera.main.ScreenToWorldPoint(mouseScreenPos); //Convierte esa posición de pantalla a coordenadas del mundo
    }

    public IEnumerator FlashRed()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null) yield break;

        Material mat = mr.material; // instancia del material
        Color originalColor = mat.color;

        Color flashColor = Color.red;

        int flashes = 3;
        float speed = 0.15f;

        for (int i = 0; i < flashes; i++)
        {
            mat.color = flashColor;
            yield return new WaitForSeconds(speed);

            mat.color = originalColor;
            yield return new WaitForSeconds(speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropArea") && tipTrigger.bounds.Intersects(other.bounds))
        {
            Debug.Log("Detecté DropArea: " + other.name);
            CurrentDropArea = other.transform;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DropArea") && tipTrigger.bounds.Intersects(other.bounds) && !placed)
        {
            Debug.Log("Detecté DropArea: " + other.name);
            CurrentDropArea = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DropArea") && CurrentDropArea == other.transform)
        {
            if (!tipTrigger.bounds.Intersects(other.bounds))
                CurrentDropArea = null;
        }
    }
}