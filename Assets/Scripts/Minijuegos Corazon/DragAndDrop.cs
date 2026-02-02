using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
<<<<<<< Updated upstream
    // ← STATIC para solo UN objeto seleccionado
=======
>>>>>>> Stashed changes
    private static DragAndDrop currentlySelected = null;

    Vector3 offset;
    public string destinationTag = "DropArea";
    public bool placed = false;
    public Transform CurrentDropArea;
    private Vector3 initialPosition;

<<<<<<< Updated upstream
    // ← SOLO referencia a TU SelectObject
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        // ← Desocupa DropArea (SI está colocado)
=======
        
>>>>>>> Stashed changes
        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;
        }

<<<<<<< Updated upstream
        // ← GESTIÓN EXCLUSIVA: Deselecciona anterior
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.selectObj?.Deselect();
        }

        // ← LLAMA a TU SelectObject.Select()
        selectObj?.Select();
        currentlySelected = this;

        // Drag
=======
        
        if (ObjectSelector.currentlySelected != null &&
            ObjectSelector.currentlySelected.gameObject != gameObject)
        {
            ObjectSelector.currentlySelected.Deselect();
        }

        ObjectSelector.currentlySelected = selectObj;
        selectObj?.Select();


>>>>>>> Stashed changes
        offset = transform.position - MouseWorldPosition();
        GetComponent<Collider>().enabled = false;
    }

<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                        selectObj?.Deselect(); // ← TU Deselect()
=======

                        ObjectSelector.currentlySelected = null;
                        selectObj?.Deselect();
>>>>>>> Stashed changes
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
