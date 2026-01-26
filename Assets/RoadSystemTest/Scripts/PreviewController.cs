using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    public static PreviewController Instance { get; private set; }    // Lo hacemos Singleton

    public GameObject[] previews;   // Vector de previews de las distintas piezas que se pueden colocar
    public GameObject parent;       // Objeto padre de las previews, utilizado para rotarlas todas al mismo tiempo

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cambia la preview para mostrar la seleccionada por el usuario
    /// </summary>
    /// <param name="selectedObject">Índice de la preview a mostrar</param>
    public void ChangePreview(int selectedObject)
    {
        // Recorre el vector, desactivando todas las previews 
        for (int i = 0; i < previews.Length; i++)
        {
            previews[i].SetActive(false);
        }
        // Activa la preview seleccionada
        previews[selectedObject].SetActive(true);
    }

    /// <summary>
    /// Rota el padre de las previews, rotando todas las previews al mismo tiempo
    /// </summary>
    /// <param name="currentRotation">Rotación a asignar</param>
    public void RotatePreview(Quaternion currentRotation)
    {
        // Le asigna la rotación al objeto
        parent.transform.rotation = currentRotation;
    }
}