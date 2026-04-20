using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private Renderer[] renderers;
    public Color selectedColor = Color.red; //color seleccionado el rojo
    public Color defaultColor = Color.white; //el color default es el blanco

    private DragAndDrop dragAndDropScriptMinigame1;
    private DragAndDropMinigame2 dragScriptMinigame2;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        dragScriptMinigame2 = GetComponentInParent<DragAndDropMinigame2>();
        SetColor(defaultColor);
    }

    public void Select() 
    {
        if (dragAndDropScriptMinigame1 != null && dragAndDropScriptMinigame1.locked) return; // ❌ No permetre seleccionar si està bloquejat
        if (dragScriptMinigame2 != null && dragScriptMinigame2.locked ) return; // ❌ No permetre seleccionar si està bloquejat

        SetColor(selectedColor);
        //Debug.Log(dragScript.name);
    }

    public void Deselect()
    {
        SetColor(defaultColor);
    }

    private void SetColor(Color color) //para setear un color al objeto seleccionado
    {
        if (renderers == null) return;
        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                r.material.color = color;
            }
        }
    }

    
}
