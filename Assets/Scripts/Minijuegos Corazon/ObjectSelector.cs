using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    private static SelectObject currentlySelected = null;
    [SerializeField]private Button rotateButton;

    private void Awake()
    {
        rotateButton.onClick.AddListener(rotatePiece);
    }

    private void rotatePiece()
    {
        if(currentlySelected != null)
        {
            currentlySelected.RotateObject();
            Debug.Log(" Rotando: " + currentlySelected.name);
        }
        
    }

    private void Update()
    {
        
        Vector3 posicionMouse = Input.mousePosition;
        Ray rayo = Camera.main.ScreenPointToRay(posicionMouse);
        RaycastHit hit;

        bool hasContact = Physics.Raycast(rayo, out hit);

        if (hasContact)
        {
             SelectObject parentSelect = hit.transform.GetComponentInParent<SelectObject>();

             if (parentSelect != null)
             {
                 // Solo selecciona si no está ya seleccionado
                 if (parentSelect != currentlySelected)
                 {
                     // Deseleccionar anterior
                     if (currentlySelected != null)
                     {
                         currentlySelected.Deselect();
                     }

                     // Seleccionar nuevo
                     parentSelect.Select();
                     currentlySelected = parentSelect;
                     Debug.Log("Seleccionado: " + parentSelect.name);
                 }
             }
        }
        
        /*if (Input.GetKeyDown(KeyCode.R) && currentlySelected != null)
        {
            currentlySelected.RotateObject();
            Debug.Log(" Rotando: " + currentlySelected.name);
        }*/

    }
}
