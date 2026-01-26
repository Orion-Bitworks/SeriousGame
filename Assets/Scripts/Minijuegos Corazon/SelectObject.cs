using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private Renderer[] renderers;
    public Color selectedColor = Color.red;
    public Color defaultColor = Color.white;

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

    private void SetColor(Color color)
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

    public void RotateObject()
    {
        transform.Rotate(0, 90f, 0);
    }
}
