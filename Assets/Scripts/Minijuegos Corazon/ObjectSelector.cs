using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public static SelectObject currentlySelected = null;

    [SerializeField]private Button rotateButton;

    static RotateObjects rotateObjectsInstance;
    //[SerializeField] private Button rotateButton;
 
    //static RotateObjects rotateObjectsInstance;//Referencia al objeto que rota

    public FasesMinigames minigamesPhasesInstance;

    private void Awake()
    {
        rotateButton.onClick.AddListener(rotatePiece);
    }

    private void rotatePiece()
    {

        if (currentlySelected == null)
        {
            Debug.Log("No se ha seleccionado ningun objeto");
            return;
        }

        rotateObjectsInstance = currentlySelected.GetComponentInParent<RotateObjects>(); //script rotateObjects

        if (rotateObjectsInstance == null)
        {
            Debug.LogError("RotateObjects NO encontrado en: " + currentlySelected.name);
            return;
        }

        Debug.Log("✅ Rotando: " + currentlySelected.name);

        if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase1Root != null &&
            minigamesPhasesInstance.fase1Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame1(currentlySelected);
            Debug.Log("Fase 1 - 180°");
        }
        else if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase2Root != null &&
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
