using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public static SelectObject currentlySelected = null;

    [SerializeField]private Button rotateButton;

    static RotateObjects rotateObjectsInstance;

    public FasesMinigames minigamesPhasesInstance;


    private void Awake()
    {
        rotateButton.onClick.AddListener(rotatePiece);
    }

    private void rotatePiece()
    {
        if (rotateObjectsInstance == null || currentlySelected == null)  // Añade esta comprobación
            return;

        // Comprueba si Fase1 está ACTIVA (no solo si existe)
        if (minigamesPhasesInstance != null &&
            minigamesPhasesInstance.fase1Root != null &&
            minigamesPhasesInstance.fase1Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame1(currentlySelected);
            Debug.Log("Fase 1 - 180°");
        }
        // Comprueba si Fase2 está ACTIVA
        else if (minigamesPhasesInstance != null &&
                 minigamesPhasesInstance.fase2Root != null &&
                 minigamesPhasesInstance.fase2Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame2(currentlySelected);
            Debug.Log("Fase 2 - 90°");
        }
    }

    /*private void Update()
    {
        
        Vector3 posicionMouse = Input.mousePosition;
        Ray rayo = Camera.main.ScreenPointToRay(posicionMouse);
        RaycastHit hit;

        bool hasContact = Physics.Raycast(rayo, out hit);

        if (hasContact)
        {
             SelectObject parentSelect = hit.transform.GetComponentInParent<SelectObject>();

             RotateObjects rotateObjects = hit.transform.GetComponentInParent<RotateObjects>();

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

                if (rotateObjects != null)
                {
                    rotateObjectsInstance = rotateObjects;
                }
            }
        }
        
        

    }*/
}
