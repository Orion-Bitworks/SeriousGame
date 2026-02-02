using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    // ← STATIC para solo UN objeto seleccionado
    private static DragAndDrop currentlySelected = null;

    Vector3 offset;
    public string destinationTag = "DropArea";
    public bool placed = false;
    public Transform CurrentDropArea;
    private Vector3 initialPosition;

    // ← SOLO referencia a TU SelectObject
    private SelectObject selectObj;

    public Minigame1 minigame1Instance;
    public Minigame2 minigame2Instance;
    public string valveType;

    private void Start()
    {
        initialPosition = transform.position;
        selectObj = GetComponent<SelectObject>();
    }

    void OnMouseDown()
    {
        // ← Desocupa DropArea (SI está colocado)
        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;
        }

        // ← GESTIÓN EXCLUSIVA: Deselecciona anterior
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.selectObj?.Deselect();
        }

        // ← LLAMA a TU SelectObject.Select()
        selectObj?.Select();
        currentlySelected = this;

        // Drag
        offset = transform.position - MouseWorldPosition();
        GetComponent<Collider>().enabled = false;
    }

    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + offset;
    }

    void OnMouseUp()
    {
        var rayOrigin = Camera.main.transform.position;
        var rayDirection = MouseWorldPosition() - Camera.main.transform.position;
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            if (hitInfo.transform.CompareTag(destinationTag))
            {
                DropArea drop = hitInfo.transform.GetComponent<DropArea>();
                if (drop != null && !drop.occupied)
                {
                    transform.position = hitInfo.transform.position;
                    CurrentDropArea = hitInfo.transform;
                    drop.occupied = true;

                    if (!placed)
                    {
                        placed = true;
                        minigame1Instance?.objectsRemaining();
                        minigame2Instance?.objectsRemaining();
                        selectObj?.Deselect(); // ← TU Deselect()
                    }
                }
                else
                {
                    transform.position = initialPosition;
                }
            }
            else
            {
                transform.position = initialPosition;
            }
        }
        else
        {
            transform.position = initialPosition;
        }

        GetComponent<Collider>().enabled = true;
    }

    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}
