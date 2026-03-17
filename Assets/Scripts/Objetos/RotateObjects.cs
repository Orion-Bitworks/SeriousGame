using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjects : MonoBehaviour
{
    public void rotateObjectsMinigame1(SelectObject objeto)  // Recibe el objeto correcto
    {
        if (objeto != null)
            objeto.transform.Rotate(0, 0, 180f, Space.Self);
    }

    public void rotateObjectsMinigame2(SelectObject objeto)
    {
        if (objeto != null)
            objeto.transform.Rotate(0, 0, 90f, Space.Self);
    }
}
