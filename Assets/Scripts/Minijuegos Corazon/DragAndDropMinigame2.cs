using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropMinigame2 : MonoBehaviour
{
    public static DragAndDropMinigame2 currentlySelected = null;

    Vector3 offset;
    public string destinationTag = "DropArea";
    public bool placed = false;
    public Transform CurrentDropArea; //No rellenar
    private Vector3 initialPosition;

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
        selectObj = GetComponent<SelectObject>();
    }

    void OnMouseDown()
    {
        if (locked) return;

        hasDragged = false;

        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;

            placed = false;
            CurrentDropArea = null;

            /*if (minigame1Instance != null && minigame1Instance.gameObject.activeSelf)
                minigame1Instance.objectsRemaining();*/

            if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                minigame2Instance.objectsRemaining();
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

        // 🔹 Solo desactivamos el collider de drag, NO el trigger
        if (dragCollider != null)
            dragCollider.enabled = false;
    }

    void OnMouseDrag()
    {
        if (locked) return;

        hasDragged = true;

        transform.position = MouseWorldPosition() + offset;
    }

    void OnMouseUp()
    {
        if (locked) return;

        if (hasDragged && CurrentDropArea != null)
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
                    //transform.rotation = drop.requiredRotation;

                    drop.occupied = true;

                    if (!placed)
                    {
                        placed = true;

                        if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                            minigame2Instance.objectsRemaining();
                    }
                    
                }
            }
        }
        else
        {
            transform.position = initialPosition;
        }

        // 🔹 Volvemos a activar el collider de drag
        if (dragCollider != null)
            dragCollider.enabled = true;
    }

    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropArea"))
        {
            Debug.Log("Detecté DropArea: " + other.name);
            CurrentDropArea = other.transform;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DropArea") && CurrentDropArea == other.transform)
            CurrentDropArea = null;
    }
}
