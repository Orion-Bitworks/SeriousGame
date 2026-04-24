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

    void OnMouseDown()
    {
        if (locked) return;

        hasDragged = false;

        // ❌ NO descolocar aquí (ya lo quitaste)
        // ❌ NO desactivar el dragCollider

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


    void OnMouseDrag()
    {
        if (locked) return;

        Vector3 newPos = MouseWorldPosition() + offset;

        // 🔥 Solo marcar como arrastrado si realmente se movió
        if (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            hasDragged = true;
        }

        // Si estaba colocada y empieza a arrastrarse → liberar DropArea
        if (hasDragged && placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;

            placed = false;
            CurrentDropArea = null;

            if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                minigame2Instance.objectsRemaining();
        }

        transform.position = newPos;


    }



    void OnMouseUp()
    {
        if (locked) return;

        // 🔹 Si NO se ha arrastrado, NO hacemos nada especial
        // (solo se seleccionó la pieza, no debe volver al inicio)
        if (!hasDragged)
        {
            if (dragCollider != null)
                dragCollider.enabled = true;
            return;
        }

        // 🔹 Si SÍ se ha arrastrado:
        if (CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();

            if (drop != null && !drop.occupied)
            {
                Transform snapPivot = CurrentDropArea.Find("SnapPivot");

                if (snapPivot != null)
                {
                    transform.position = snapPivot.position;
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
            // Si la pieza se arrastra pero no esta en una drop area, vuelve a su sitioo
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            placed = false;
            CurrentDropArea = null;
        }

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
        if (!hasDragged) return;

        if (other.CompareTag("DropArea") && CurrentDropArea == other.transform)
            CurrentDropArea = null;
    }
}
