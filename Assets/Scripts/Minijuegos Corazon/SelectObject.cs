using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private Renderer[] renderers;
    public Color selectedColor = Color.red; //color seleccionado el rojo
    public Color defaultColor = Color.white; //el color default es el blanco

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetColor(defaultColor);
    }

    public void Select() 
    {
        SetColor(selectedColor);
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
