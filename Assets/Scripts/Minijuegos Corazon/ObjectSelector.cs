using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    private static SelectObject currentlySelected = null;

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
                        Debug.Log("✅ Seleccionado: " + parentSelect.name);
                    }
                }
            }
        
        
    }
}
